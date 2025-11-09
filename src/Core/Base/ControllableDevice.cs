using System;
using WaterTreatmentSCADA.Core.Interfaces;

namespace WaterTreatmentSCADA.Core.Base
{
    /// <summary>
    /// Base class for devices that can be controlled by operators
    /// </summary>
    public abstract class ControllableDevice : BaseDevice
    {
        public bool IsRunning => isRunning;
        
        protected ControllableDevice(string name, string deviceType, string simulationFilePath)
            : base(name, deviceType, simulationFilePath)
        {
        }
        
        public abstract void SetConfig(string configName, object value);
        public abstract object GetConfig(string configName);
        
        public virtual void TurnOn()
        {
            isRunning = true;
            Status = DeviceStatus.Online;
            Console.WriteLine($"{Name} turned ON");
        }
        
        public virtual void TurnOff()
        {
            isRunning = false;
            Status = DeviceStatus.Offline;
            Console.WriteLine($"{Name} turned OFF");
        }
    }
}
