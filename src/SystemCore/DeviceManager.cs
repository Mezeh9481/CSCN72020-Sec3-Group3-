using System;
using System.Threading;
using WaterTreatmentSCADA.Core.Interfaces;

namespace WaterTreatmentSCADA.SystemCore
{
    /// <summary>
    /// Central manager for all devices
    /// </summary>
    public class DeviceManager
    {
        private IDeviceRepository repository;
        private Timer updateTimer;
        private int updateIntervalMs = 1000;
        
        public DeviceManager(IDeviceRepository repository)
        {
            this.repository = repository ?? throw new ArgumentNullException(nameof(repository));
        }
        
        public void InitializeAllDevices()
        {
            foreach (var device in repository.GetAllDevices())
            {
                device.Initialize();
            }
            Console.WriteLine("All devices initialized");
        }
        
        public void StartAllDevices()
        {
            foreach (var device in repository.GetAllDevices())
            {
                device.Start();
            }
            
            updateTimer = new Timer(UpdateAllDevicesCallback, null, 0, updateIntervalMs);
            Console.WriteLine("All devices started");
        }
        
        public void StopAllDevices()
        {
            updateTimer?.Dispose();
            
            foreach (var device in repository.GetAllDevices())
            {
                device.Stop();
            }
            Console.WriteLine("All devices stopped");
        }
        
        public void UpdateAllDevices()
        {
            foreach (var device in repository.GetAllDevices())
            {
                try
                {
                    device.Update();
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error updating {device.Name}: {ex.Message}");
                }
            }
        }
        
        private void UpdateAllDevicesCallback(object state)
        {
            UpdateAllDevices();
        }
        
        public IDevice GetDevice(string deviceName)
        {
            return repository.GetDevice(deviceName);
        }
        
        public void PrintSystemTelemetry()
        {
            Console.WriteLine("\n=== SYSTEM TELEMETRY ===");
            foreach (var device in repository.GetAllDevices())
            {
                var telemetry = device.GetTelemetryData();
                Console.WriteLine($"\n{device.Name}:");
                foreach (var kvp in telemetry)
                {
                    Console.WriteLine($"  {kvp.Key}: {kvp.Value}");
                }
            }
            Console.WriteLine("========================\n");
        }
    }
}
