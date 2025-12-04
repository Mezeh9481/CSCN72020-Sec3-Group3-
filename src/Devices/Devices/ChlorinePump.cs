using System;
using System.Collections.Generic;
using WaterTreatmentSCADA.Core.Base;
using WaterTreatmentSCADA.Core.Interfaces;

namespace WaterTreatmentSCADA.Devices.Devices
{
    // Chlorine pump that controls chlorine dosing into treatment system
    // Supports on/off control and variable dosing rate (0-100%)
    // Also monitors chlorine level from simulation
    public class ChlorinePump : ControllableDevice
    {
        public bool IsOn { get; private set; }
        public double DosingRate { get; private set; }
        public double ChlorineLevel { get; private set; }

        public event EventHandler<bool>? OnStateChange;
        public event EventHandler<double>? OnDosingRateChange;
        public event EventHandler<double>? OnChlorineLevelChange;

        private const double MinDosingRate = 0.0;
        private const double MaxDosingRate = 100.0;
        private bool previousState;
        private double previousDosingRate;
        private double previousChlorineLevel;

        public ChlorinePump(string name, string simulationFilePath)
            : base(name, "ChlorinePump", simulationFilePath)
        {
            IsOn = false;
            DosingRate = 0.0;
            ChlorineLevel = 0.0;
            previousState = IsOn;
            previousDosingRate = DosingRate;
            previousChlorineLevel = ChlorineLevel;
        }

        // Turn pump on
        public void TurnOn()
        {
            if (!IsOn)
            {
                IsOn = true;
                isRunning = true;
                Status = DeviceStatus.Online;
                previousState = IsOn;
                OnStateChange?.Invoke(this, true);
                Console.WriteLine($"{Name} turned ON");
            }
        }

        // Turn pump off
        public void TurnOff()
        {
            if (IsOn)
            {
                IsOn = false;
                isRunning = false;
                DosingRate = 0.0; // Reset dosing rate
                Status = DeviceStatus.Offline;
                previousState = IsOn;
                OnStateChange?.Invoke(this, false);
                OnDosingRateChange?.Invoke(this, 0.0);
                Console.WriteLine($"{Name} turned OFF");
            }
        }

        // Set dosing rate percentage (0-100)
        public void SetDosingRate(double dosingRate)
        {
            if (dosingRate < MinDosingRate || dosingRate > MaxDosingRate)
            {
                throw new ArgumentOutOfRangeException(nameof(dosingRate), 
                    $"Dosing rate must be between {MinDosingRate} and {MaxDosingRate}%");
            }

            previousDosingRate = DosingRate;
            DosingRate = dosingRate;
            LastUpdate = DateTime.Now;

            // Update status
            if (DosingRate > 0 && IsOn)
            {
                Status = DeviceStatus.Online;
            }
            else if (!IsOn)
            {
                Status = DeviceStatus.Offline;
            }

            // Fire event if dosing rate changed significantly (avoid floating point noise)
            if (Math.Abs(DosingRate - previousDosingRate) > 0.1)
            {
                OnDosingRateChange?.Invoke(this, DosingRate);
            }
        }

        // Update pump state from CSV file (called every 1 second by DeviceManager)
        public override void Update()
        {
            if (!isRunning)
                return;

            try
            {
                string? dataLine = GetFileInfo();

                if (!string.IsNullOrEmpty(dataLine))
                {
                    // Parse CSV: timestamp,chlorineLevel,dosingRate,isRunning,status
                    var parts = dataLine.Split(',');
                    if (parts.Length >= 4)
                    {
                        // Update chlorine level (column 1)
                        if (double.TryParse(parts[1], out double chlorineLevel))
                        {
                            if (Math.Abs(chlorineLevel - ChlorineLevel) > 0.01)
                            {
                                previousChlorineLevel = ChlorineLevel;
                                ChlorineLevel = chlorineLevel;
                                OnChlorineLevelChange?.Invoke(this, ChlorineLevel);
                            }
                        }

                        // Update dosing rate (column 2)
                        if (double.TryParse(parts[2], out double dosingRate))
                        {
                            // Convert from raw value to percentage (assuming 0-1 range to 0-100%)
                            dosingRate = dosingRate * 100.0;
                            dosingRate = Math.Max(MinDosingRate, Math.Min(MaxDosingRate, dosingRate));
                            
                            if (Math.Abs(dosingRate - DosingRate) > 0.1)
                            {
                                previousDosingRate = DosingRate;
                                DosingRate = dosingRate;
                                OnDosingRateChange?.Invoke(this, DosingRate);
                            }
                        }

                        // Update on/off state (column 3)
                        if (bool.TryParse(parts[3], out bool isRunning))
                        {
                            previousState = IsOn;
                            IsOn = isRunning;
                            this.isRunning = isRunning;

                            if (IsOn != previousState)
                            {
                                OnStateChange?.Invoke(this, IsOn);
                            }

                            // Update status based on state
                            if (IsOn)
                            {
                                Status = DeviceStatus.Online;
                            }
                            else
                            {
                                Status = DeviceStatus.Offline;
                                DosingRate = 0.0;
                            }
                        }

                        // Handle status column (column 4) for warnings/critical
                        if (parts.Length >= 5)
                        {
                            string statusStr = parts[4].Trim().ToLower();
                            if (statusStr == "critical" || statusStr == "error")
                            {
                                Status = DeviceStatus.Error;
                            }
                            else if (statusStr == "warning")
                            {
                                Status = DeviceStatus.Warning;
                            }
                        }

                        LastUpdate = DateTime.Now;
                    }
                }
            }
            catch (Exception ex)
            {
                Status = DeviceStatus.Error;
                Console.WriteLine($"Error updating {Name}: {ex.Message}");
            }
        }

        // Set configuration parameter
        public override void SetConfig(string configName, object value)
        {
            switch (configName.ToLower())
            {
                case "dosingrate":
                    if (value is double doubleValue)
                    {
                        SetDosingRate(doubleValue);
                    }
                    else if (value is int intValue)
                    {
                        SetDosingRate(intValue);
                    }
                    break;
                case "ison":
                case "on":
                    if (value is bool boolValue)
                    {
                        if (boolValue)
                            TurnOn();
                        else
                            TurnOff();
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
                "dosingrate" => DosingRate,
                "chlorinelevel" => ChlorineLevel,
                "ison" => IsOn,
                "on" => IsOn,
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
                { "isOn", IsOn },
                { "dosingRate", DosingRate },
                { "chlorineLevel", ChlorineLevel },
                { "isRunning", IsRunning },
                { "lastUpdate", LastUpdate.ToString("yyyy-MM-dd HH:mm:ss") }
            };
        }
    }
}