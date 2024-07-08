using Android.Content;
using Android.OS;
using Android.Provider;
using Android.Views;
using MauiCamera2.Services;

namespace MauiCamera2.Platforms.Droid.Services
{
    /// <summary>
    /// 安卓平台服务
    /// </summary>
    public class PlatformService : IPlatformService
    {
        //在Android 10及以上版本，由于引入了Scoped Storage的概念，直接访问外部存储的根目录可能会受到限制
        private string _rootDir = Path.Combine(Android.OS.Environment.ExternalStorageDirectory?.Path ??
            Android.App.Application.Context.GetExternalFilesDir(type: Android.OS.Environment.DirectoryDocuments)?.Path ?? "", ".berrypis", "v1");

        public async Task CheckPermissions()
        {
            PermissionStatus status = PermissionStatus.Unknown; 

            var cameraStatus = await Permissions.CheckStatusAsync<Permissions.Camera>();
            if (cameraStatus != PermissionStatus.Granted)
            {
                cameraStatus = await Permissions.RequestAsync<Permissions.Camera>();
            }

            if (cameraStatus != PermissionStatus.Granted)
            {
                await Shell.Current.DisplayAlert("错误", "未取得相机权限，程序将自动退出!", "OK");
                System.Environment.Exit(0);
                return;
            }

            var audioStatus = await Permissions.CheckStatusAsync<Permissions.Microphone>();
            if (audioStatus != PermissionStatus.Granted)
            {
                audioStatus = await Permissions.RequestAsync<Permissions.Microphone>();
            }

            if (audioStatus != PermissionStatus.Granted)
            {
                await   Shell.Current.DisplayAlert("错误", "未取得麦克风权限，程序将自动退出!", "OK");
                System.Environment.Exit(0);
                return;
            }

            var storageStatus = await Permissions.CheckStatusAsync<Permissions.StorageWrite>();
            if (storageStatus != PermissionStatus.Granted)
            {
                storageStatus = await Permissions.RequestAsync<Permissions.StorageWrite>();
            }

            if (storageStatus != PermissionStatus.Granted)
            {
                await Shell.Current.DisplayAlert("错误", "未取得文件读写权限，程序将自动退出!", "OK");
                System.Environment.Exit(0);
                return;
            }

#pragma warning disable CA1416 // 验证平台兼容性
            //权限确认
            if (Build.VERSION.SdkInt >= BuildVersionCodes.M)
            {
                if (!Settings.CanDrawOverlays(Platform.AppContext))
                {

                    //启动Activity让用户授权
                    Intent intent = new Intent(Settings.ActionManageOverlayPermission);
                    intent.SetData(Android.Net.Uri.Parse("package:" + Platform.CurrentActivity?.PackageName ?? ""));
                    Platform.CurrentActivity?.StartActivityForResult(intent, 0);
                    var ret = await Shell.Current.DisplayAlert("提示", "已授予浮窗权限?", "确认", "取消");
                    if (!ret)
                    {
                        System.Environment.Exit(0);
                    }
                }
                if (!Settings.CanDrawOverlays(Platform.AppContext))
                {
                    await Shell.Current.DisplayAlert("错误", "未取得浮窗权限，程序将自动退出!", "确认");
                    System.Environment.Exit(0);
                }
            }
#pragma warning restore CA1416 // 验证平台兼容性

            if (Build.VERSION.SdkInt >= BuildVersionCodes.M)
            {
                status = await Permissions.CheckStatusAsync<ReadWriteStoragePerms>();
                if (status != PermissionStatus.Granted)
                    status = await Permissions.RequestAsync<ReadWriteStoragePerms>();
                if (status != PermissionStatus.Granted)
                {
                    await Shell.Current.DisplayAlert("错误", "未取得文件权限，程序将自动退出!", "确认");
                    System.Environment.Exit(0);
                }

                

            }
            if (Build.VERSION.SdkInt >= BuildVersionCodes.R)
            {
#pragma warning disable CA1416 // 验证平台兼容性
                if (!Android.OS.Environment.IsExternalStorageManager)
                {
                    Intent intent = new Intent(Android.Provider.Settings.ActionManageAppAllFilesAccessPermission);
                    intent.SetData(Android.Net.Uri.Parse("package:" + Platform.CurrentActivity?.PackageName ?? ""));
                    Platform.CurrentActivity?.StartActivityForResult(intent, 0);
                    var ret = await Shell.Current.DisplayAlert("提示", "已授予文件权限?", "确认", "取消");
                    if (!ret)
                    {
                        System.Environment.Exit(0);
                    }
                }
                if (Android.OS.Environment.IsExternalStorageManager) status = PermissionStatus.Granted;
                else
                {
                    await Shell.Current.DisplayAlert("错误", "未取得文件权限，程序将自动退出!", "确认");
                    System.Environment.Exit(0);
                }
#pragma warning restore CA1416 // 验证平台兼容性
            }
            try
            {
                if (status == PermissionStatus.Granted)

                    //已授权，卸载不会删掉关键数据
                    _rootDir = Path.Combine(Android.OS.Environment.ExternalStorageDirectory?.Path ?? "", ".berrypis", "v1");
                else
                    //未授权，则使用程序内部的存储，卸载会删掉数据
                    _rootDir = Android.App.Application.Context.GetExternalFilesDir(type: Android.OS.Environment.DirectoryDocuments)?.Path ?? "";
                if (!Directory.Exists(_rootDir))
                    Directory.CreateDirectory(_rootDir);
            }
            catch (Exception)
            {
                _rootDir = Android.App.Application.Context.GetExternalFilesDir(type: Android.OS.Environment.DirectoryDocuments)?.Path ?? "";
            }


        }


        public string GetRootPath()
        {
            if (!Directory.Exists(_rootDir))
                Directory.CreateDirectory(_rootDir);
            return _rootDir;
        }

        /// <summary>
        /// 打开系统设置
        /// </summary>
        public void OpenTimeSetting()
        {
            Intent intent = new(Settings.ActionDateSettings);
            intent.AddFlags(ActivityFlags.NewTask);
            Platform.CurrentActivity?.StartActivityForResult(intent, 0);
        }

        /// <summary>
        /// 获取文件
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public byte[]? GetFile(string filePath)
        {
            string path = Path.Combine(_rootDir, filePath);
            try
            {
                if (!File.Exists(path)) return null;
                return File.ReadAllBytes(path);
            }
            catch (Exception)
            {
                return null;
            }
        }
        /// <summary>
        /// 保存文件
        /// </summary>
        /// <param name="dFile"></param>
        /// <param name="filePath"></param>
        public void SaveFile(byte[] dFile, string filePath)
        {
            string path = Path.Combine(_rootDir, filePath);
            try
            {
                if (File.Exists(path))
                {
                    File.Delete(path);
                }
                if (!Directory.Exists(Path.GetDirectoryName(path)))
                {
                    Directory.CreateDirectory(Path.GetDirectoryName(path) ?? "");
                }
                File.WriteAllBytes(path, dFile);

            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("文件保存错误:" + ex.Message);
            }
        }
    }
}
