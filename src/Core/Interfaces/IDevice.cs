using System;
using System.Collections.Generic;

namespace WaterTreatmentSCADA.Core.Interfaces
{
    public interface IDevice
    {
        string Name { get; }
        string DeviceType { get; }
        DeviceStatus Status { get; }
        DateTime LastUpdate { get; }

        void Initialize();
        void Start();
        void Stop();
        void Update();
        Dictionary<string, object> GetTelemetryData();
    }
}
