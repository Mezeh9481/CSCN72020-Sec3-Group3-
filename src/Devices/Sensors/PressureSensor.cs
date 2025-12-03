using System;
using System.Collections.Generic;
using WaterTreatmentSCADA.Core.Base;
using WaterTreatmentSCADA.Core.Interfaces;

namespace WaterTreatmentSCADA.Devices.Sensors
{
    // Pressure sensor that monitors pipeline and filter pressure
    // Reads simulated values from CSV file
    // Emits events when readings change (updates every 1 second)
    // Pressure range: 0 - 5.0 bar
    public class PressureSensor : BaseDevice
    {
        public double CurrentReading { get; private set; } // Current pressure in bar
        public event EventHandler<double>? OnReadingChange;

        private double previousReading;

        // Pressure thresholds (bar) - based on CSV data patterns
        public const double MinSafePressure = 1.5;    // Below this = potential pump failure/leak
        public const double NormalPressureLow = 2.0;  // Normal operating range start
        public const double NormalPressureHigh = 2.6; // Normal operating range end
        public const double WarningPressure = 2.7;    // Elevated pressure (monitor closely)
        public const double CriticalPressure = 3.0;   // Critical pressure (possible blockage)
        public const double MaxPressure = 5.0;        // Absolute maximum

        public PressureSensor(string name, string simulationFilePath)
            : base(name, "PressureSensor", simulationFilePath)
        {
            CurrentReading = 2.3; // Start at normal pressure
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
                    // Parse CSV: timestamp,pressure,location,status
                    var parts = dataLine.Split(',');
                    if (parts.Length >= 2 && double.TryParse(parts[1], out double pressureValue))
                    {
                        // Clamp to sensor range (0-5 bar)
                        pressureValue = Math.Max(0.0, Math.Min(MaxPressure, pressureValue));

                        previousReading = CurrentReading;
                        CurrentReading = pressureValue;
                        LastUpdate = DateTime.Now;

                        UpdateStatus(pressureValue);

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

        // Update status based on pressure value
        private void UpdateStatus(double pressureValue)
        {
            if (pressureValue < MinSafePressure)
            {
                // Very low pressure - potential leak or pump failure
                Status = DeviceStatus.Critical;
            }
            else if (pressureValue >= MinSafePressure && pressureValue < NormalPressureLow)
            {
                // Low but not critical
                Status = DeviceStatus.Warning;
            }
            else if (pressureValue >= NormalPressureLow && pressureValue <= NormalPressureHigh)
            {
                // Normal operating range (2.0-2.6 bar)
                Status = DeviceStatus.Online;
            }
            else if (pressureValue > NormalPressureHigh && pressureValue < CriticalPressure)
            {
                // Elevated pressure (2.7-2.9 bar) - monitor closely
                Status = DeviceStatus.Warning;
            }
            else // pressureValue >= CriticalPressure (≥3.0 bar)
            {
                // Critical high pressure - possible blockage
                Status = DeviceStatus.Critical;
            }
        }

        // Get pressure status description for UI
        public string GetPressureStatusDescription()
        {
            if (CurrentReading < MinSafePressure)
                return "CRITICAL LOW: Check for leaks or pump failure";
            else if (CurrentReading < NormalPressureLow)
                return "Low Pressure";
            else if (CurrentReading <= NormalPressureHigh)
                return "Normal Pressure";
            else if (CurrentReading < CriticalPressure)
                return "Elevated Pressure";
            else
                return "CRITICAL HIGH: Possible blockage";
        }

        // Return telemetry data for debugging/monitoring
        public override Dictionary<string, object> GetTelemetryData()
        {
            return new Dictionary<string, object>
            {
                { "name", Name },
                { "type", DeviceType },
                { "status", Status.ToString() },
                { "pressureReading", CurrentReading },
                { "pressureUnit", "bar" },
                { "pressureStatus", GetPressureStatusDescription() },
                { "isRunning", isRunning },
                { "lastUpdate", LastUpdate.ToString("yyyy-MM-dd HH:mm:ss") }
            };
        }
    }
}