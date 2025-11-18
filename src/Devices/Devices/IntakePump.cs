using System;
using System.Collections.Generic;
using WaterTreatmentSCADA.Core.Base;
using WaterTreatmentSCADA.Core.Interfaces;

namespace WaterTreatmentSCADA.Devices.Devices
{
    // Intake pump that controls water flow into treatment system
    // Supports on/off control and variable flow rate (0-100%)
    public class IntakePump : ControllableDevice
    {
        public bool IsOn { get; private set; }
        public double FlowRate { get; private set; }

        public event EventHandler<bool>? OnStateChange;
        public event EventHandler<double>? OnFlowRateChange;

        private const double MinFlowRate = 0.0;
        private const double MaxFlowRate = 100.0;
        private bool previousState;
        private double previousFlowRate;

        public IntakePump(string name, string simulationFilePath)
            : base(name, "IntakePump", simulationFilePath)
        {
            IsOn = false;
            FlowRate = 0.0;
            previousState = IsOn;
            previousFlowRate = FlowRate;
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
                FlowRate = 0.0; // Reset flow rate
                Status = DeviceStatus.Offline;
                previousState = IsOn;
                OnStateChange?.Invoke(this, false);
                OnFlowRateChange?.Invoke(this, 0.0);
                Console.WriteLine($"{Name} turned OFF");
            }
        }

        // Set flow rate percentage (0-100)
        public void SetFlowRate(double flowRate)
        {
            if (flowRate < MinFlowRate || flowRate > MaxFlowRate)
            {
                throw new ArgumentOutOfRangeException(nameof(flowRate), 
                    $"Flow rate must be between {MinFlowRate} and {MaxFlowRate}%");
            }

            previousFlowRate = FlowRate;
            FlowRate = flowRate;
            LastUpdate = DateTime.Now;

            // Update status
            if (FlowRate > 0 && IsOn)
            {
                Status = DeviceStatus.Online;
            }
            else if (!IsOn)
            {
                Status = DeviceStatus.Offline;
            }

            // Fire event if flow rate changed significantly (avoid floating point noise)
            if (Math.Abs(FlowRate - previousFlowRate) > 0.1)
            {
                OnFlowRateChange?.Invoke(this, FlowRate);
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
                    // Parse CSV: timestamp,flowRate,isRunning,pressure,status
                    var parts = dataLine.Split(',');
                    if (parts.Length >= 3)
                    {
                        // Update flow rate
                        if (double.TryParse(parts[1], out double flowRate))
                        {
                            flowRate = Math.Max(MinFlowRate, Math.Min(MaxFlowRate, flowRate));
                            
                            if (Math.Abs(flowRate - FlowRate) > 0.1)
                            {
                                previousFlowRate = FlowRate;
                                FlowRate = flowRate;
                                OnFlowRateChange?.Invoke(this, FlowRate);
                            }
                        }

                        // Update on/off state
                        if (bool.TryParse(parts[2], out bool isRunning))
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
                                FlowRate = 0.0;
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
                case "flowrate":
                    if (value is double doubleValue)
                    {
                        SetFlowRate(doubleValue);
                    }
                    else if (value is int intValue)
                    {
                        SetFlowRate(intValue);
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
                "flowrate" => FlowRate,
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
                { "flowRate", FlowRate },
                { "isRunning", IsRunning },
                { "lastUpdate", LastUpdate.ToString("yyyy-MM-dd HH:mm:ss") }
            };
        }
    }
}

