using System.Collections.Generic;

namespace WaterTreatmentSCADA.Core.Interfaces
{
    public interface IDeviceRepository
    {
        List<IDevice> Devices { get; }
        IDevice GetDevice(string deviceName);
        void AddDevice(IDevice device);
        void RemoveDevice(string deviceName);
        List<IDevice> GetAllDevices();
    }
}
