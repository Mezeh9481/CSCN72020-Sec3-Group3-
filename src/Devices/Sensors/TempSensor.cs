using System;
using System.Collections.Generic;
using WaterTreatmentSCADA.Core.Base;
using WaterTreatmentSCADA.Core.Interfaces;

namespace WaterTreatmentSCADA.Devices.Sensors
{
    // pH sensor that reads simulated values from CSV file
    // Emits events when readings change (updates every 1 second)
    // pH range: 5.0 - 9.0
    public class TempSensor : BaseDevice
    {
        public double CurrentReading { get; private set; }
        public event EventHandler<double>? OnReadingChange;

        private const double MinTemp = 18.0;
        private const double CriticalTemp = 24.0;
        private double previousReading;

        public TempSensor(string name, string simulationFilePath)
            : base(name, "TempSensor", simulationFilePath)
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
                    // Parse CSV: timestamp,TempValue,status
                    var parts = dataLine.Split(',');
                    if (parts.Length >= 2 && double.TryParse(parts[1], out double TempValue))
                    {
                        // Make sure value is in valid range
                        TempValue = Math.Max(MinTemp, Math.Min(CriticalTemp, TempValue));

                        previousReading = CurrentReading;
                        CurrentReading = TempValue;
                        LastUpdate = DateTime.Now;

                        UpdateStatus(TempValue);

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

        // Update status based on Temp value
        private void UpdateStatus(double TempValue)
        {
            if (TempValue >=22 || TempValue <23)
            {
                Status = DeviceStatus.Warning;
            }
            else if (TempValue >= 23)
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

