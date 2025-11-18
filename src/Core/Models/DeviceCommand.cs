using System;

namespace WaterTreatmentSCADA.Core.Models
{
    // Command to be executed on a device
    public class DeviceCommand
    {
        public string DeviceName { get; set; }
        public string Command { get; set; }
        public object[] Parameters { get; set; }
        public DateTime Timestamp { get; set; }
        
        public DeviceCommand(string deviceName, string command, params object[] parameters)
        {
            DeviceName = deviceName;
            Command = command;
            Parameters = parameters;
            Timestamp = DateTime.Now;
        }
        
        public override string ToString()
        {
            return $"[{Timestamp:HH:mm:ss}] {DeviceName}: {Command}";
        }
    }
}
