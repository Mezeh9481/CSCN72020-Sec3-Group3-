using System;
using System.Collections.Generic;
using WaterTreatmentSCADA.Core.Base;
using WaterTreatmentSCADA.Core.Interfaces;

namespace WaterTreatmentSCADA.Devices.Sensors
{
    // Water storage that reads simulated values from CSV file
    // Emits events when readings change (updates every 1 second)
    public class StorageSensor : BaseDevice
    {
        public double CurrentReading { get; private set; }
        public event EventHandler<double>? OnReadingChange;
        private const double MinStorage = 0.0;
        private const double CriticalStorage = 950.0;
        private const double MaxStorage = 1000;
        private double previousReading;

        public StorageSensor(string name, string simulationFilePath)
            : base(name, "StorageSensor", simulationFilePath)
        {
            CurrentReading = 2000; // Start at neutral Storage
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
                    if (parts.Length >= 2 && double.TryParse(parts[1], out double StorageValue))
                    {
                        // Make sure value is in valid range

                        previousReading = CurrentReading;
                        CurrentReading = StorageValue;
                        LastUpdate = DateTime.Now;

                        UpdateStatus(StorageValue);

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
        private void UpdateStatus(double StorageValue)
        {
            if (StorageValue >= 950 && StorageValue <= 1000)
            {
                Status = DeviceStatus.Warning;
            }
            else if (StorageValue > 1000)
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
                { "storage sensor", CurrentReading },
                { "isRunning", isRunning },
                { "lastUpdate", LastUpdate.ToString("yyyy-MM-dd HH:mm:ss") }
            };
        }
    }
}

