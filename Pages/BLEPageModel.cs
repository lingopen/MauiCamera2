using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MauiCamera2.Models;
using Plugin.BLE;
using Plugin.BLE.Abstractions.Contracts;
using Plugin.BLE.Abstractions.Extensions;
using System.Collections.ObjectModel;
using System.Diagnostics;
using ScanMode = Plugin.BLE.Abstractions.Contracts.ScanMode;
namespace MauiCamera2.Pages
{
    public partial class BLEPageModel : BaseViewModel
    {
        private readonly IBluetoothLE _bluetoothManager;
        private readonly IAdapter _bluetoothAdapter;
        private bool _isScanning = false;
        public bool IsScanning
        {
            get => _isScanning;
            protected set
            {
                if (_isScanning != value)
                {
                    _isScanning = value;
                    OnPropertyChanged(nameof(IsScanning));
                    OnPropertyChanged(nameof(Waiting));
                    OnPropertyChanged(nameof(ScanState));
                    OnPropertyChanged(nameof(ToggleScanningCmdLabelText));
                }
            }
        }

        public bool IsStateOn => _bluetoothManager.IsOn;
        public string StateText => GetStateText();
        public bool Waiting => !_isScanning;
        public string ScanState => IsScanning ? "Scanning" : "Waiting";
        public string ToggleScanningCmdLabelText => IsScanning ? "Cancel" : "Start Scan";
        private string GetStateText()
        {
            var result = "Unknown BLE state.";
            switch (_bluetoothManager.State)
            {
                case BluetoothState.Unknown:
                    result = "Unknown BLE state.";
                    break;
                case BluetoothState.Unavailable:
                    result = "BLE is not available on this device.";
                    break;
                case BluetoothState.Unauthorized:
                    result = "You are not allowed to use BLE.";
                    break;
                case BluetoothState.TurningOn:
                    result = "BLE is warming up, please wait.";
                    break;
                case BluetoothState.On:
                    result = "BLE is on.";
                    break;
                case BluetoothState.TurningOff:
                    result = "BLE is turning off. That's sad!";
                    break;
                case BluetoothState.Off:
                    result = "BLE is off. Turn it on!";
                    break;
            }
            return result;
        }
        CancellationTokenSource _scanCancellationTokenSource = null;
        public BLEPageModel() : base()
        {
            _bluetoothManager = CrossBluetoothLE.Current;
            _bluetoothAdapter = _bluetoothManager.Adapter;
            DeviceList = new ObservableCollection<DeviceItem>();
            if (_bluetoothManager is null)
            {
                Shell.Current.DisplayAlert("错误", "BluetoothManager is null", "确定");

            }
            else if (_bluetoothAdapter is null)
            {
                Shell.Current.DisplayAlert("错误", "Adapter is null", "确定");
            }
            else
            {
                ConfigureBLE();
            }
        }
        private void ConfigureBLE()
        {
            _bluetoothManager.StateChanged += _bluetoothManager_StateChanged;

            // Set up scanner
            _bluetoothAdapter.ScanMode = ScanMode.LowLatency;
            _bluetoothAdapter.ScanTimeout = 30000; // ms
            _bluetoothAdapter.ScanTimeoutElapsed += _bluetoothAdapter_ScanTimeoutElapsed;
            _bluetoothAdapter.DeviceAdvertised += _bluetoothAdapter_DeviceAdvertised;
            _bluetoothAdapter.DeviceDiscovered += _bluetoothAdapter_DeviceDiscovered;
            DebugMessage("Configuring BLE... DONE");
        }
        [RelayCommand]
        void ToggleScanning()
        {
            if (!IsScanning)
            {
                IsScanning = true;
                DebugMessage($"Starting Scanning");
                ScanForDevicesAsync();
                DebugMessage($"Started Scan");
            }
            else
            {
                DebugMessage($"Request Stopping Scan");
                _scanCancellationTokenSource?.Cancel();
                DebugMessage($"Stop Scanning Requested");
            }
        }

        private async void ScanForDevicesAsync()
        {
            if (!IsStateOn)
            {
                ShowMessage("Bluetooth is not ON.\nPlease turn on Bluetooth and try again.");
                IsScanning = false;
                return;
            }
            if (!await HasCorrectPermissions())
            {
                DebugMessage("Aborting scan attempt");
                IsScanning = false;
                return;
            }
            DebugMessage("StartScanForDevices called");
            DeviceList.Clear();
            await UpdateConnectedDevices();

            _scanCancellationTokenSource = new();

            DebugMessage("call Adapter.StartScanningForDevicesAsync");
            await _bluetoothAdapter.StartScanningForDevicesAsync(_scanCancellationTokenSource.Token);
            DebugMessage("back from Adapter.StartScanningForDevicesAsync");

            // Scanning stopped (for whichever reason) -> cleanup
            _scanCancellationTokenSource.Dispose();
            _scanCancellationTokenSource = null;
            IsScanning = false;
        }
        private async Task<bool> HasCorrectPermissions()
        {
            DebugMessage("Verifying Bluetooth permissions..");
            var permissionResult = await Permissions.CheckStatusAsync<Permissions.Bluetooth>();
            if (permissionResult != PermissionStatus.Granted)
            {
                permissionResult = await Permissions.RequestAsync<Permissions.Bluetooth>();
            }
            DebugMessage($"Result of requesting Bluetooth permissions: '{permissionResult}'");
            if (permissionResult != PermissionStatus.Granted)
            {
                DebugMessage("Permissions not available, direct user to settings screen.");
                ShowMessage("Permission denied. Not scanning.");
                AppInfo.ShowSettingsUI();
                return false;
            }

            return true;
        }
        private async Task UpdateConnectedDevices()
        {
            foreach (var connectedDevice in _bluetoothAdapter.ConnectedDevices)
            {
                //update rssi for already connected devices (so that 0 is not shown in the list)
                try
                {
                    await connectedDevice.UpdateRssiAsync();
                }
                catch (Exception ex)
                {
                    ShowMessage($"Failed to update RSSI for {connectedDevice.Name}. Error: {ex.Message}");
                }

                AddOrUpdateDevice(connectedDevice);
            }
        }
        private void AddOrUpdateDevice(IDevice device)
        {
            MainThread.BeginInvokeOnMainThread(() =>
            {
                var vm = DeviceList.FirstOrDefault(d => d.DeviceId == device.Id);
                if (vm != null)
                {
                    DebugMessage($"Update Device: {device.Id}");
                    vm.Update(device);
                }
                else
                {
                    DebugMessage($"Add Device: {device.Id}");
                    vm = new DeviceItem(device);
                    DeviceList.Add(vm);
                }
            });
        }
        private void DebugMessage(string message)
        {
            Debug.WriteLine(message);
        }
        private void ShowMessage(string message)
        {
            DebugMessage(message);
            Shell.Current.DisplayAlert("错误", message, "确定");
        }
        private void _bluetoothManager_StateChanged(object? sender, Plugin.BLE.Abstractions.EventArgs.BluetoothStateChangedArgs e)
        {
            OnPropertyChanged(nameof(IsStateOn));
            OnPropertyChanged(nameof(StateText));
        }

        private void _bluetoothAdapter_DeviceAdvertised(object? sender, Plugin.BLE.Abstractions.EventArgs.DeviceEventArgs e)
        {
            DebugMessage("OnDeviceAdvertised");
            AddOrUpdateDevice(e.Device);
            DebugMessage("OnDeviceAdvertised done");
        }

        private void _bluetoothAdapter_ScanTimeoutElapsed(object? sender, EventArgs e)
        {
            Console.WriteLine("Adapter_ScanTimeoutElapsed");
        }

        /// <summary>
        /// 新设备连接
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <exception cref="NotImplementedException"></exception>
        private void _bluetoothAdapter_DeviceDiscovered(object? sender, Plugin.BLE.Abstractions.EventArgs.DeviceEventArgs e)
        {
            DebugMessage("OnDeviceDiscovered");
            AddOrUpdateDevice(e.Device);
            DebugMessage("OnDeviceDiscovered done");
        }
        [ObservableProperty]
        ObservableCollection<DeviceItem> deviceList;

    }
}
