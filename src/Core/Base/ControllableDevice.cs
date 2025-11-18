using System;
using WaterTreatmentSCADA.Core.Interfaces;

namespace WaterTreatmentSCADA.Core.Base
{
    // Base class for devices that can be controlled (pumps, valves, etc.)
    public abstract class ControllableDevice : BaseDevice
    {
        public bool IsRunning => isRunning;
        
        protected ControllableDevice(string name, string deviceType, string simulationFilePath)
            : base(name, deviceType, simulationFilePath)
        {
        }
        
        // Set configuration parameter
        public abstract void SetConfig(string configName, object value);
        
        // Get configuration parameter value
        public abstract object? GetConfig(string configName);
        
        // Turn device on
        public virtual void TurnOn()
        {
            isRunning = true;
            Status = DeviceStatus.Online;
            Console.WriteLine($"{Name} turned ON");
        }
        
        // Turn device off
        public virtual void TurnOff()
        {
            isRunning = false;
            Status = DeviceStatus.Offline;
            Console.WriteLine($"{Name} turned OFF");
        }
    }
}
