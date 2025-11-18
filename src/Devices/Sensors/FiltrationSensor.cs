using System;
using System.Collections.Generic;
using WaterTreatmentSCADA.Core.Base;
using WaterTreatmentSCADA.Core.Interfaces;

namespace WaterTreatmentSCADA.Devices.Sensors
{
    // Filtration sensor that measures turbidity (NTU)
    // Emits events when turbidity changes and alerts when > 5 NTU
    public class FiltrationSensor : BaseDevice
    {
        public double CurrentTurbidity { get; private set; }
        public double AlertThreshold { get; set; }
        public bool IsAlertActive { get; private set; }

        public event EventHandler<double>? OnTurbidityChange;
        public event EventHandler<double>? OnThresholdAlert;
        public event EventHandler<double>? OnThresholdCleared;

        private const double MinTurbidity = 0.0;
        private const double MaxTurbidity = 10.0;
        private const double DefaultAlertThreshold = 5.0;
        private double previousTurbidity;
        private bool previousAlertState;

        public FiltrationSensor(string name, string simulationFilePath, double alertThreshold = DefaultAlertThreshold)
            : base(name, "FiltrationSensor", simulationFilePath)
        {
            CurrentTurbidity = 0.0;
            AlertThreshold = alertThreshold;
            IsAlertActive = false;
            previousTurbidity = CurrentTurbidity;
            previousAlertState = IsAlertActive;
        }

        // Update reading from CSV file (called every 1 second by DeviceManager)
        public override void Update()
        {
            if (!isRunning)
                return;

            try
            {
                string? dataLine = GetFileInfo();

                if (!string.IsNullOrEmpty(dataLine))
                {
                    // Parse CSV: timestamp,turbidity,status
                    var parts = dataLine.Split(',');
                    if (parts.Length >= 2 && double.TryParse(parts[1], out double turbidity))
                    {
                        // Make sure value is in valid range
                        turbidity = Math.Max(MinTurbidity, Math.Min(MaxTurbidity, turbidity));

                        previousTurbidity = CurrentTurbidity;
                        CurrentTurbidity = turbidity;
                        LastUpdate = DateTime.Now;

                        UpdateStatus(turbidity);
                        CheckThresholdAlerts(turbidity);

                        // Only fire event if reading actually changed (avoid floating point noise)
                        if (Math.Abs(CurrentTurbidity - previousTurbidity) > 0.01)
                        {
                            OnTurbidityChange?.Invoke(this, CurrentTurbidity);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Status = DeviceStatus.Error;
                Console.WriteLine($"Error updating {Name}: {ex.Message}");
            }
        }

        // Update status based on turbidity value
        private void UpdateStatus(double turbidity)
        {
            if (turbidity > AlertThreshold)
            {
                Status = DeviceStatus.Warning;
            }
            else if (turbidity > MaxTurbidity * 0.9) // > 9.0 NTU = critical
            {
                Status = DeviceStatus.Critical;
            }
            else
            {
                Status = DeviceStatus.Online;
            }
        }

        // Check if turbidity exceeds threshold and fire alert events
        private void CheckThresholdAlerts(double turbidity)
        {
            previousAlertState = IsAlertActive;
            IsAlertActive = turbidity > AlertThreshold;

            // Alert just triggered
            if (IsAlertActive && !previousAlertState)
            {
                OnThresholdAlert?.Invoke(this, turbidity);
                Console.WriteLine($"⚠️ ALERT: {Name} turbidity ({turbidity:F2} NTU) exceeds threshold ({AlertThreshold} NTU)");
            }
            // Alert just cleared
            else if (!IsAlertActive && previousAlertState)
            {
                OnThresholdCleared?.Invoke(this, turbidity);
                Console.WriteLine($"✅ ALERT CLEARED: {Name} turbidity ({turbidity:F2} NTU) is now below threshold ({AlertThreshold} NTU)");
            }
        }

        // Return telemetry data for debugging/monitoring
        public override Dictionary<string, object> GetTelemetryData()
        {
            return new Dictionary<string, object>
            {
                { "name", Name },
                { "type", DeviceType },
                { "status", Status.ToString() },
                { "turbidity", CurrentTurbidity },
                { "alertThreshold", AlertThreshold },
                { "isAlertActive", IsAlertActive },
                { "isRunning", isRunning },
                { "lastUpdate", LastUpdate.ToString("yyyy-MM-dd HH:mm:ss") }
            };
        }
    }
}

