using System;
using System.Collections.Generic;
using System.IO;
using WaterTreatmentSCADA.Core.Interfaces;
using WaterTreatmentSCADA.Devices.Devices;
using WaterTreatmentSCADA.Devices.Sensors;

namespace WaterTreatmentSCADA.SystemCore
{
    // Central controller that manages all sensors and devices
    // Handles events and updates GUI
    public class SystemController
    {
        // System-wide constants
        public const double LowerSafePH = 6.5;
        public const double UpperSafePH = 8.5;
        public const double MinPH = 5.0;
        public const double MaxPH = 9.0;

        public const double GreenTurbidityThreshold = 3.0;  // < 3 NTU = safe
        public const double YellowTurbidityThreshold = 5.0; // 3-5 NTU = warning
        public const double RedTurbidityThreshold = 5.0;    // > 5 NTU = alert
        public const double MaxTurbidity = 10.0;

        public const double MinFlowRate = 0.0;
        public const double MaxFlowRate = 100.0;

        public const int UpdateIntervalMs = 1000;

        // Device instances
        private pHSensor? phSensor;
        private FiltrationSensor? filtrationSensor;
        private IntakePump? intakePump;
        private ChemicalDoser? chemicalDoser;
        private DeviceRepository? deviceRepository;
        private DeviceManager? deviceManager;

        // Events for GUI updates
        public event EventHandler<double>? OnPHReadingChanged;
        public event EventHandler<bool>? OnChemicalDoserStateChanged;
        public event EventHandler<bool>? OnIntakePumpStateChanged;
        public event EventHandler<double>? OnIntakePumpFlowRateChanged;
        public event EventHandler<double>? OnFiltrationTurbidityChanged;
        public event EventHandler<double>? OnFiltrationAlert;
        public event EventHandler<double>? OnFiltrationAlertCleared;
        public event EventHandler<SystemEvent>? OnSystemEvent;

        // Public access to devices
        public pHSensor? PHSensor => phSensor;
        public FiltrationSensor? FiltrationSensor => filtrationSensor;
        public IntakePump? IntakePump => intakePump;
        public ChemicalDoser? ChemicalDoser => chemicalDoser;
        public DeviceManager? DeviceManager => deviceManager;
        public bool IsRunning { get; private set; }

        // Initialize the system with simulation data path
        public void Initialize(string simulationDataPath)
        {
            try
            {
                LogEvent("SystemController", "Initializing system...", SystemEventType.Info);

                deviceRepository = new DeviceRepository();

                // Create sensors
                phSensor = new pHSensor(
                    "Main pH Sensor",
                    Path.Combine(simulationDataPath, "pHSensor_simulation.csv")
                );

                filtrationSensor = new FiltrationSensor(
                    "Filtration Sensor",
                    Path.Combine(simulationDataPath, "TurbiditySensor_simulation.csv"),
                    RedTurbidityThreshold
                );

                // Create devices
                intakePump = new IntakePump(
                    "Main Intake Pump",
                    Path.Combine(simulationDataPath, "IntakePump_simulation.csv")
                );

                chemicalDoser = new ChemicalDoser("Chemical Doser");

                // Link chemical doser to pH sensor for automatic activation
                if (chemicalDoser != null && phSensor != null)
                {
                    chemicalDoser.SetPHSensor(phSensor);
                }

                // Register all devices
                if (phSensor != null)
                    deviceRepository.AddDevice(phSensor);
                if (filtrationSensor != null)
                    deviceRepository.AddDevice(filtrationSensor);
                if (intakePump != null)
                    deviceRepository.AddDevice(intakePump);
                if (chemicalDoser != null)
                    deviceRepository.AddDevice(chemicalDoser);

                // Setup device manager
                deviceManager = new DeviceManager(deviceRepository);
                deviceManager.InitializeAllDevices();

                // Subscribe to device events
                SubscribeToDeviceEvents();

                LogEvent("SystemController", "System initialized successfully", SystemEventType.Info);
            }
            catch (Exception ex)
            {
                LogEvent("SystemController", $"Initialization error: {ex.Message}", SystemEventType.Error);
                throw;
            }
        }

        // Subscribe to all device events so we can broadcast to GUI
        private void SubscribeToDeviceEvents()
        {
            // Subscribe to pH sensor events
            if (phSensor != null)
            {
                phSensor.OnReadingChange += OnPHSensorReadingChanged;
            }

            // Subscribe to chemical doser events
            if (chemicalDoser != null)
            {
                chemicalDoser.OnStateChange += OnChemicalDoserStateChangedInternal;
            }

            // Subscribe to intake pump events
            if (intakePump != null)
            {
                intakePump.OnStateChange += OnIntakePumpStateChangedInternal;
                intakePump.OnFlowRateChange += OnIntakePumpFlowRateChangedInternal;
            }

            // Subscribe to filtration sensor events
            if (filtrationSensor != null)
            {
                filtrationSensor.OnTurbidityChange += OnFiltrationTurbidityChangedInternal;
                filtrationSensor.OnThresholdAlert += OnFiltrationAlertInternal;
                filtrationSensor.OnThresholdCleared += OnFiltrationAlertClearedInternal;
            }

            LogEvent("EventSubscription", "All device events subscribed", SystemEventType.Info);
        }

        // Event handlers - receive events from devices and broadcast to GUI
        private void OnPHSensorReadingChanged(object? sender, double phValue)
        {
            LogEvent("pHSensor", $"pH reading changed: {phValue:F2}", SystemEventType.DataUpdate);

            // Check if pH is out of safe range
            bool isSafe = phValue >= LowerSafePH && phValue <= UpperSafePH;
            if (!isSafe)
            {
                LogEvent("pHSensor", $"⚠️ pH OUT OF RANGE: {phValue:F2} (safe range: {LowerSafePH}-{UpperSafePH})", SystemEventType.Warning);
            }

            // Chemical doser handles activation automatically (already linked to pH sensor)
            // This method can add more automatic actions if needed

            // Broadcast to GUI
            OnPHReadingChanged?.Invoke(this, phValue);
        }

        private void OnChemicalDoserStateChangedInternal(object? sender, bool isActive)
        {
            LogEvent("ChemicalDoser", $"State changed: {(isActive ? "ACTIVE" : "INACTIVE")}", SystemEventType.StateChange);
            OnChemicalDoserStateChanged?.Invoke(this, isActive);
        }

        private void OnIntakePumpStateChangedInternal(object? sender, bool isOn)
        {
            LogEvent("IntakePump", $"State changed: {(isOn ? "ON" : "OFF")}", SystemEventType.StateChange);
            OnIntakePumpStateChanged?.Invoke(this, isOn);
        }

        private void OnIntakePumpFlowRateChangedInternal(object? sender, double flowRate)
        {
            LogEvent("IntakePump", $"Flow rate changed: {flowRate:F1}%", SystemEventType.DataUpdate);
            OnIntakePumpFlowRateChanged?.Invoke(this, flowRate);
        }

        private void OnFiltrationTurbidityChangedInternal(object? sender, double turbidity)
        {
            LogEvent("FiltrationSensor", $"Turbidity changed: {turbidity:F2} NTU", SystemEventType.DataUpdate);

            if (turbidity >= RedTurbidityThreshold)
            {
                LogEvent("FiltrationSensor", $"⚠️ HIGH TURBIDITY: {turbidity:F2} NTU (threshold: {RedTurbidityThreshold} NTU)", SystemEventType.Warning);
            }

            OnFiltrationTurbidityChanged?.Invoke(this, turbidity);
        }

        private void OnFiltrationAlertInternal(object? sender, double turbidity)
        {
            LogEvent("FiltrationSensor", $"🚨 ALERT: Turbidity threshold exceeded! Value: {turbidity:F2} NTU", SystemEventType.Alert);
            OnFiltrationAlert?.Invoke(this, turbidity);
        }

        private void OnFiltrationAlertClearedInternal(object? sender, double turbidity)
        {
            LogEvent("FiltrationSensor", $"✅ ALERT CLEARED: Turbidity back to normal. Value: {turbidity:F2} NTU", SystemEventType.Info);
            OnFiltrationAlertCleared?.Invoke(this, turbidity);
        }

        // Control methods for user actions
        public void TurnOnIntakePump()
        {
            if (intakePump != null)
            {
                intakePump.TurnOn();
                LogEvent("UserAction", "Intake pump turned ON", SystemEventType.UserAction);
            }
        }

        public void TurnOffIntakePump()
        {
            if (intakePump != null)
            {
                intakePump.TurnOff();
                LogEvent("UserAction", "Intake pump turned OFF", SystemEventType.UserAction);
            }
        }

        public void SetIntakePumpFlowRate(double flowRate)
        {
            if (intakePump != null)
            {
                try
                {
                    intakePump.SetFlowRate(flowRate);
                    LogEvent("UserAction", $"Intake pump flow rate set to {flowRate:F1}%", SystemEventType.UserAction);
                }
                catch (ArgumentOutOfRangeException ex)
                {
                    LogEvent("UserAction", $"Error setting flow rate: {ex.Message}", SystemEventType.Error);
                    throw;
                }
            }
        }

        public void ActivateChemicalDoser()
        {
            if (chemicalDoser != null)
            {
                chemicalDoser.Activate();
                LogEvent("UserAction", "Chemical doser manually ACTIVATED", SystemEventType.UserAction);
            }
        }

        public void DeactivateChemicalDoser()
        {
            if (chemicalDoser != null)
            {
                chemicalDoser.Deactivate();
                LogEvent("UserAction", "Chemical doser manually DEACTIVATED", SystemEventType.UserAction);
            }
        }

        // System control
        public void Start()
        {
            if (deviceManager != null && !IsRunning)
            {
                deviceManager.StartAllDevices();
                IsRunning = true;
                LogEvent("SystemController", "System STARTED", SystemEventType.Info);
            }
        }

        public void Stop()
        {
            if (deviceManager != null && IsRunning)
            {
                deviceManager.StopAllDevices();
                IsRunning = false;
                LogEvent("SystemController", "System STOPPED", SystemEventType.Info);
            }
        }

        public void Shutdown()
        {
            Stop();
            UnsubscribeFromDeviceEvents();
            LogEvent("SystemController", "System SHUTDOWN", SystemEventType.Info);
        }

        private void UnsubscribeFromDeviceEvents()
        {
            if (phSensor != null)
            {
                phSensor.OnReadingChange -= OnPHSensorReadingChanged;
            }

            if (chemicalDoser != null)
            {
                chemicalDoser.OnStateChange -= OnChemicalDoserStateChangedInternal;
            }

            if (intakePump != null)
            {
                intakePump.OnStateChange -= OnIntakePumpStateChangedInternal;
                intakePump.OnFlowRateChange -= OnIntakePumpFlowRateChangedInternal;
            }

            if (filtrationSensor != null)
            {
                filtrationSensor.OnTurbidityChange -= OnFiltrationTurbidityChangedInternal;
                filtrationSensor.OnThresholdAlert -= OnFiltrationAlertInternal;
                filtrationSensor.OnThresholdCleared -= OnFiltrationAlertClearedInternal;
            }
        }

        // Logging
        private void LogEvent(string source, string message, SystemEventType eventType)
        {
            var systemEvent = new SystemEvent
            {
                Timestamp = DateTime.Now,
                Source = source,
                Message = message,
                EventType = eventType
            };

            // Print to console
            Console.WriteLine($"[{systemEvent.Timestamp:yyyy-MM-dd HH:mm:ss}] [{systemEvent.EventType}] [{source}] {message}");

            // Raise event in case GUI wants to log it
            OnSystemEvent?.Invoke(this, systemEvent);
        }

        // Get current system state (for debugging/monitoring)
        public Dictionary<string, object> GetSystemTelemetry()
        {
            var telemetry = new Dictionary<string, object>
            {
                { "isRunning", IsRunning },
                { "timestamp", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") }
            };

            if (phSensor != null)
            {
                telemetry["pHReading"] = phSensor.CurrentReading;
            }

            if (filtrationSensor != null)
            {
                telemetry["turbidity"] = filtrationSensor.CurrentTurbidity;
                telemetry["turbidityAlert"] = filtrationSensor.IsAlertActive;
            }

            if (intakePump != null)
            {
                telemetry["intakePumpOn"] = intakePump.IsOn;
                telemetry["intakePumpFlowRate"] = intakePump.FlowRate;
            }

            if (chemicalDoser != null)
            {
                telemetry["chemicalDoserActive"] = chemicalDoser.IsActive;
            }

            return telemetry;
        }
    }

    // Helper class for logging events
    public class SystemEvent
    {
        public DateTime Timestamp { get; set; }
        public string Source { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public SystemEventType EventType { get; set; }
    }

    // Types of events we can log
    public enum SystemEventType
    {
        Info,
        DataUpdate,
        StateChange,
        Warning,
        Alert,
        Error,
        UserAction,
        AutomaticAction
    }
}
