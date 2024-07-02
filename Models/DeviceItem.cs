using Plugin.BLE.Abstractions;
using Plugin.BLE.Abstractions.Contracts;
using PropertyChanged;
using System.Text;

namespace MauiCamera2.Models
{
    [AddINotifyPropertyChangedInterface]
    public class DeviceItem
    {
        public bool IsConnectable { get; set; }
        public Guid DeviceId { get; set; }
        public string Name { get; set; }
        public int Rssi { get; set; }
        public IDevice Device { get; set; }
        public DeviceState State { get; set;}
        public IReadOnlyList<AdvertisementRecord> AdvertisementRecords;
        public string Adverts
        {
            get => String.Join('\n', AdvertisementRecords.Select(advert => $"{advert.Type}: 0x{Convert.ToHexString(advert.Data)}"));
        }
        public DeviceItem(IDevice device)
        {
            Update(device);
        }
        public void Update(IDevice device)
        {
            DeviceId = device.Id;
            Name = device.Name;
            Rssi = device.Rssi;
            IsConnectable = device.IsConnectable;
            AdvertisementRecords = device.AdvertisementRecords;
            State = device.State;
        }
        public override string ToString()
        {
            var advertData = new StringBuilder();
            foreach (var advert in AdvertisementRecords)
            {
                advertData.Append($"|{advert.Type}: 0x{Convert.ToHexString(advert.Data)}|");
            }

            return $"{Name}:{DeviceId}: Adverts: '{advertData}'";
        }
    }

}
