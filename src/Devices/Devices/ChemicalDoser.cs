using System;
using System.Collections.Generic;
using WaterTreatmentSCADA.Core.Base;
using WaterTreatmentSCADA.Core.Interfaces;
using WaterTreatmentSCADA.Devices.Sensors;

namespace WaterTreatmentSCADA.Devices.Devices
{
    // Chemical doser that automatically activates when pH is out of range
    // Activates when pH < 6.5 or pH > 8.5
    public class ChemicalDoser : ControllableDevice
    {
        public bool IsActive { get; private set; }
        public event EventHandler<bool>? OnStateChange;

        private const double LowerPHThreshold = 6.5;
        private const double UpperPHThreshold = 8.5;
        private pHSensor? phSensor;

        public ChemicalDoser(string name, string? simulationFilePath = null)
            : base(name, "ChemicalDoser", simulationFilePath ?? "")
        {
            IsActive = false;
        }

        // Link pH sensor for automatic activation
        public void SetPHSensor(pHSensor sensor)
        {
            // Unsubscribe from old sensor if there was one
            if (phSensor != null)
            {
                phSensor.OnReadingChange -= OnPHSensorReadingChanged;
            }

            phSensor = sensor;
            
            // Subscribe to new sensor
            if (phSensor != null)
            {
                phSensor.OnReadingChange += OnPHSensorReadingChanged;
            }
        }

        // Handle pH reading changes - activate/deactivate automatically
        private void OnPHSensorReadingChanged(object? sender, double phValue)
        {
            if (phValue < LowerPHThreshold || phValue > UpperPHThreshold)
            {
                // pH out of range - activate
                if (!IsActive)
                {
                    Activate();
                }
            }
            else
            {
                // pH in normal range - deactivate
                if (IsActive)
                {
                    Deactivate();
                }
            }
        }

        // Manually activate doser
        public void Activate()
        {
            if (!IsActive)
            {
                IsActive = true;
                Status = DeviceStatus.Online;
                OnStateChange?.Invoke(this, true);
                Console.WriteLine($"{Name} ACTIVATED");
            }
        }

        // Manually deactivate doser
        public void Deactivate()
        {
            if (IsActive)
            {
                IsActive = false;
                Status = DeviceStatus.Online;
                OnStateChange?.Invoke(this, false);
                Console.WriteLine($"{Name} DEACTIVATED");
            }
        }

        // Update device state (mostly controlled by pH sensor events)
        public override void Update()
        {
            if (!isRunning)
                return;

            LastUpdate = DateTime.Now;
        }

        // Set configuration parameter
        public override void SetConfig(string configName, object value)
        {
            switch (configName.ToLower())
            {
                case "active":
                    if (value is bool boolValue)
                    {
                        if (boolValue)
                            Activate();
                        else
                            Deactivate();
                    }
                    break;
                default:
                    Console.WriteLine($"Unknown config parameter: {configName}");
                    break;
            }
        }

        // Get configuration parameter value
        public override object? GetConfig(string configName)
        {
            return configName.ToLower() switch
            {
                "active" => IsActive,
                "isactive" => IsActive,
                _ => null
            };
        }

        // Return telemetry data for debugging/monitoring
        public override Dictionary<string, object> GetTelemetryData()
        {
            return new Dictionary<string, object>
            {
                { "name", Name },
                { "type", DeviceType },
                { "status", Status.ToString() },
                { "isActive", IsActive },
                { "isRunning", IsRunning },
                { "lastUpdate", LastUpdate.ToString("yyyy-MM-dd HH:mm:ss") }
            };
        }
    }
}

