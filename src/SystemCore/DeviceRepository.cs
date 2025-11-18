using System;
using System.Collections.Generic;
using System.Linq;
using WaterTreatmentSCADA.Core.Interfaces;

namespace WaterTreatmentSCADA.SystemCore
{
    // Repository for storing and managing all devices
    public class DeviceRepository : IDeviceRepository
    {
        public List<IDevice> Devices { get; private set; }
        
        public DeviceRepository()
        {
            Devices = new List<IDevice>();
        }
        
        public IDevice GetDevice(string deviceName)
        {
            return Devices.FirstOrDefault(d => d.Name.Equals(deviceName, StringComparison.OrdinalIgnoreCase));
        }
        
        public void AddDevice(IDevice device)
        {
            if (device == null)
                throw new ArgumentNullException(nameof(device));
                
            if (GetDevice(device.Name) != null)
                throw new InvalidOperationException($"Device with name '{device.Name}' already exists");
                
            Devices.Add(device);
            Console.WriteLine($"Device added to repository: {device.Name}");
        }
        
        public void RemoveDevice(string deviceName)
        {
            var device = GetDevice(deviceName);
            if (device != null)
            {
                Devices.Remove(device);
                Console.WriteLine($"Device removed from repository: {deviceName}");
            }
        }
        
        public List<IDevice> GetAllDevices()
        {
            return new List<IDevice>(Devices);
        }
    }
}
