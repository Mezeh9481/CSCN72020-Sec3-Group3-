using System;
using System.Collections.Generic;
using WaterTreatmentSCADA.Core.Interfaces;
using WaterTreatmentSCADA.Core.Utilities;

namespace WaterTreatmentSCADA.Core.Base
{
    /// <summary>
    /// Abstract base class for all devices
    /// </summary>
    public abstract class BaseDevice : IDevice
    {
        public string Name { get; protected set; }
        public string DeviceType { get; protected set; }
        public DeviceStatus Status { get; protected set; }
        public DateTime LastUpdate { get; protected set; }
        
        protected IFileSimulator fileSimulator;
        protected bool isRunning;
        
        protected BaseDevice(string name, string deviceType, string simulationFilePath)
        {
            Name = name;
            DeviceType = deviceType;
            Status = DeviceStatus.Offline;
            isRunning = false;
            
            if (!string.IsNullOrEmpty(simulationFilePath))
            {
                fileSimulator = new BinaryIO(simulationFilePath);
            }
        }
        
        protected string GetFileInfo()
        {
            if (fileSimulator != null && !fileSimulator.IsEndOfFile)
            {
                return fileSimulator.ReadLine();
            }
            else if (fileSimulator != null)
            {
                fileSimulator.Reset();
                return fileSimulator.ReadLine();
            }
            return null;
        }
        
        public virtual void Initialize()
        {
            Status = DeviceStatus.Online;
            LastUpdate = DateTime.Now;
        }
        
        public virtual void Start()
        {
            isRunning = true;
            Status = DeviceStatus.Online;
        }
        
        public virtual void Stop()
        {
            isRunning = false;
            Status = DeviceStatus.Offline;
        }
        
        public abstract void Update();
        public abstract Dictionary<string, object> GetTelemetryData();
    }
}
