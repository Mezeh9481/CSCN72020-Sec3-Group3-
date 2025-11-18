using System;
using System.Collections.Generic;
using WaterTreatmentSCADA.Core.Base;
using WaterTreatmentSCADA.Core.Interfaces;

namespace WaterTreatmentSCADA.Devices.Sensors
{
    // pH sensor that reads simulated values from CSV file
    // Emits events when readings change (updates every 1 second)
    // pH range: 5.0 - 9.0
    public class pHSensor : BaseDevice
    {
        public double CurrentReading { get; private set; }
        public event EventHandler<double>? OnReadingChange;

        private const double MinPH = 5.0;
        private const double MaxPH = 9.0;
        private double previousReading;

        public pHSensor(string name, string simulationFilePath)
            : base(name, "pHSensor", simulationFilePath)
        {
            CurrentReading = 7.0; // Start at neutral pH
            previousReading = CurrentReading;
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
                    // Parse CSV: timestamp,phValue,status
                    var parts = dataLine.Split(',');
                    if (parts.Length >= 2 && double.TryParse(parts[1], out double phValue))
                    {
                        // Make sure value is in valid range
                        phValue = Math.Max(MinPH, Math.Min(MaxPH, phValue));

                        previousReading = CurrentReading;
                        CurrentReading = phValue;
                        LastUpdate = DateTime.Now;

                        UpdateStatus(phValue);

                        // Only fire event if reading actually changed (avoid floating point noise)
                        if (Math.Abs(CurrentReading - previousReading) > 0.01)
                        {
                            OnReadingChange?.Invoke(this, CurrentReading);
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

        // Update status based on pH value
        private void UpdateStatus(double phValue)
        {
            if (phValue < 6.5 || phValue > 8.5)
            {
                Status = DeviceStatus.Warning;
            }
            else if (phValue < 6.0 || phValue > 9.0)
            {
                Status = DeviceStatus.Critical;
            }
            else
            {
                Status = DeviceStatus.Online;
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
                { "phReading", CurrentReading },
                { "isRunning", isRunning },
                { "lastUpdate", LastUpdate.ToString("yyyy-MM-dd HH:mm:ss") }
            };
        }
    }
}

