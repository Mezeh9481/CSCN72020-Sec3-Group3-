# SystemController Integration Layer

## Overview

The `SystemController` class provides a centralized integration layer that coordinates all sensors, devices, and GUI components. It manages device lifecycle, event subscriptions, automatic actions, and state broadcasting.

## File Path

**Location:** `src/SystemCore/SystemController.cs`

## Architecture

### SystemController Responsibilities

1. **Device Instantiation**: Creates and configures all sensors and devices
2. **Event Subscription**: Subscribes to all device events centrally
3. **Event Broadcasting**: Broadcasts device events to GUI components
4. **Global Constants**: Contains all system-wide constants (pH ranges, turbidity limits, etc.)
5. **Automatic Actions**: Handles automatic device responses to sensor readings
6. **Control Methods**: Provides interface for user actions to control devices
7. **Logging**: Logs all system events for debugging and monitoring
8. **System Lifecycle**: Manages system start/stop/shutdown

## Global Constants

All system-wide constants are defined as `public const` in `SystemController`:

```csharp
// pH monitoring
public const double LowerSafePH = 6.5;
public const double UpperSafePH = 8.5;
public const double MinPH = 5.0;
public const double MaxPH = 9.0;

// Turbidity monitoring
public const double GreenTurbidityThreshold = 3.0;   // < 3 NTU = Good
public const double YellowTurbidityThreshold = 5.0;  // 3-5 NTU = Warning
public const double RedTurbidityThreshold = 5.0;     // > 5 NTU = Alert
public const double MaxTurbidity = 10.0;

// Flow rate
public const double MinFlowRate = 0.0;
public const double MaxFlowRate = 100.0;

// Update interval
public const int UpdateIntervalMs = 1000;
```

## Event Flow Architecture

### Data Flow Diagram

```
┌─────────────────────────────────────────────────────────────────────────┐
│                         DATA FLOW ARCHITECTURE                           │
└─────────────────────────────────────────────────────────────────────────┘

SENSORS → CONTROLLER → GUI → USER ACTIONS → DEVICES
   │          │          │          │            │
   │          │          │          │            │
   ▼          ▼          ▼          ▼            ▼

┌─────────┐    ┌──────────────────┐    ┌──────────────┐    ┌─────────┐
│ pHSensor│───▶│ SystemController │───▶│ GUI Panels   │───▶│ User    │
│         │    │                  │    │              │    │ Actions │
│ Reading │    │  • Event         │    │  • pH Panel  │    │  • Turn │
│ Change  │    │    Broadcasting  │    │  • Pump Panel│    │    On   │
│         │    │  • Automatic     │    │  • Filter    │    │  • Set  │
└─────────┘    │    Actions       │    │    Panel     │    │    Flow │
               │  • Logging       │    │              │    │         │
┌─────────┐    │                  │    │              │    │         │
│Filtration│───▶│                  │    │              │    │         │
│Sensor   │    │                  │    │              │    │         │
│         │    │                  │    └──────────────┘    │         │
│Turbidity│    │                  │           │             │         │
│Change   │    │                  │           │             │         │
└─────────┘    └──────────────────┘           │             │         │
                                              │             │         │
                                              ▼             ▼         ▼
                                       ┌──────────────────────────────┐
                                       │    Device Control Methods    │
                                       │                              │
                                       │  • TurnOnIntakePump()        │
                                       │  • TurnOffIntakePump()       │
                                       │  • SetIntakePumpFlowRate()   │
                                       │  • ActivateChemicalDoser()   │
                                       └──────────────────────────────┘
                                                      │
                                                      ▼
                                               ┌──────────────┐
                                               │   Devices    │
                                               │              │
                                               │  • IntakePump│
                                               │  • Chemical  │
                                               │    Doser     │
                                               └──────────────┘
```

### Detailed Event Flow

#### 1. Sensor Reading Flow

```
pHSensor.Update() [DeviceManager timer]
    ↓
pHSensor detects reading change
    ↓
pHSensor.OnReadingChange event raised
    ↓
SystemController.OnPHSensorReadingChanged() handler
    ↓
  ├─▶ LogEvent() - Logs to console/system log
  ├─▶ HandlePHReading() - Triggers automatic actions if needed
  └─▶ OnPHReadingChanged?.Invoke() - Broadcasts to GUI
       ↓
    PHMonitoringPanel.UpdatePHDisplay() [via Dispatcher.Invoke]
       ↓
    UI updated with new pH value and color coding
```

#### 2. Automatic Action Flow

```
pHSensor reading: pH = 6.2 (out of range)
    ↓
SystemController.OnPHSensorReadingChanged()
    ↓
ChemicalDoser.OnPHSensorReadingChanged() [already subscribed to pH sensor]
    ↓
ChemicalDoser.Activate() [automatic - pH < 6.5]
    ↓
ChemicalDoser.OnStateChange event raised
    ↓
SystemController.OnChemicalDoserStateChangedInternal()
    ↓
  ├─▶ LogEvent() - Logs activation
  └─▶ OnChemicalDoserStateChanged?.Invoke() - Broadcasts to GUI
       ↓
    PHMonitoringPanel.UpdateDoserStatus() [via Dispatcher.Invoke]
       ↓
    UI shows "ACTIVE" status with red background
```

#### 3. User Action Flow

```
User clicks "TURN ON" button in IntakePumpPanel
    ↓
IntakePumpPanel.PowerButton_Click()
    ↓
IntakePump.TurnOn() [direct device call]
    ↓
IntakePump.OnStateChange event raised
    ↓
SystemController.OnIntakePumpStateChangedInternal()
    ↓
  ├─▶ LogEvent() - Logs user action
  └─▶ OnIntakePumpStateChanged?.Invoke() - Broadcasts to GUI
       ↓
    IntakePumpPanel.UpdatePowerButton() [via Dispatcher.Invoke]
       ↓
    UI shows "TURN OFF" button, enables flow rate slider
```

**Alternative (via SystemController):**
```
User action → SystemController.TurnOnIntakePump()
    ↓
IntakePump.TurnOn()
    ↓
[Same event flow as above]
```

## Usage Example

### Initialization in MainWindow

```csharp
// Create and initialize system controller
systemController = new SystemController();
systemController.Initialize(dataPath);

// Subscribe to system controller events (optional)
systemController.OnSystemEvent += OnSystemEventReceived;

// Initialize panels with device references
PhMonitoringPanel.Initialize(
    systemController.PHSensor!, 
    systemController.ChemicalDoser!
);
IntakePumpPanel.Initialize(systemController.IntakePump!);
FiltrationSensorPanel.Initialize(systemController.FiltrationSensor!);

// Start the system
systemController.Start();
```

### Using Control Methods

```csharp
// Turn on intake pump
systemController.TurnOnIntakePump();

// Set flow rate
systemController.SetIntakePumpFlowRate(75.0);

// Manually activate chemical doser
systemController.ActivateChemicalDoser();
```

### Subscribing to Events

```csharp
// Subscribe to pH reading changes
systemController.OnPHReadingChanged += (sender, phValue) =>
{
    Console.WriteLine($"pH updated: {phValue:F2}");
};

// Subscribe to system events for logging
systemController.OnSystemEvent += (sender, e) =>
{
    // Handle system events (Info, Warning, Alert, Error, etc.)
    if (e.EventType == SystemEventType.Alert)
    {
        // Show alert dialog, send notification, etc.
    }
};
```

## Event Types

The `SystemController` broadcasts the following events:

### Sensor Events
- `OnPHReadingChanged(EventArgs, double phValue)` - pH reading changed
- `OnFiltrationTurbidityChanged(EventArgs, double turbidity)` - Turbidity changed
- `OnFiltrationAlert(EventArgs, double turbidity)` - Turbidity alert threshold exceeded
- `OnFiltrationAlertCleared(EventArgs, double turbidity)` - Turbidity alert cleared

### Device State Events
- `OnChemicalDoserStateChanged(EventArgs, bool isActive)` - Doser active/inactive
- `OnIntakePumpStateChanged(EventArgs, bool isOn)` - Pump on/off
- `OnIntakePumpFlowRateChanged(EventArgs, double flowRate)` - Flow rate changed

### System Events
- `OnSystemEvent(EventArgs, SystemEvent e)` - System events for logging
  - Event types: `Info`, `DataUpdate`, `StateChange`, `Warning`, `Alert`, `Error`, `UserAction`, `AutomaticAction`

## Automatic Actions

The `SystemController` handles automatic device responses:

### pH-Based Actions
- **Chemical Doser**: Automatically activates when pH < 6.5 or pH > 8.5
- The doser is linked to the pH sensor via `ChemicalDoser.SetPHSensor()`
- Deactivates automatically when pH returns to safe range (6.5 - 8.5)

### Future Automatic Actions
The `HandlePHReading()` method can be extended to add more automatic responses:
- Automatically reduce intake pump flow rate during pH adjustment
- Trigger additional filtration cycles when pH is unstable
- Send notifications when pH remains out of range for extended period

## Logging

All system events are logged via `SystemController.LogEvent()`:

- **Console Output**: All events are logged to console with timestamp, type, source, and message
- **Event Broadcasting**: `OnSystemEvent` event can be subscribed for custom logging (file, database, UI log viewer, etc.)

### Log Format
```
[2025-01-15 14:23:45] [Info] [SystemController] System initialized successfully
[2025-01-15 14:23:46] [DataUpdate] [pHSensor] pH reading changed: 7.25
[2025-01-15 14:23:47] [Warning] [pHSensor] ⚠️ pH OUT OF RANGE: 6.20 (safe range: 6.5-8.5)
[2025-01-15 14:23:47] [StateChange] [ChemicalDoser] State changed: ACTIVE
[2025-01-15 14:23:48] [Alert] [FiltrationSensor] 🚨 ALERT: Turbidity threshold exceeded! Value: 5.50 NTU
```

## Thread Safety

- All event handlers in `SystemController` run on the device thread
- GUI updates must use `Dispatcher.Invoke()` (already handled in panels)
- Device control methods are thread-safe and can be called from UI thread

## System Lifecycle

```csharp
// 1. Initialize
systemController = new SystemController();
systemController.Initialize(dataPath);

// 2. Start
systemController.Start();  // Starts all devices and update timer

// 3. Runtime
// Events flow automatically between sensors → controller → GUI

// 4. Stop
systemController.Stop();  // Stops all devices

// 5. Shutdown
systemController.Shutdown();  // Cleans up all resources and unsubscribes events
```

## Integration with Existing Code

### Device Manager
The `SystemController` uses `DeviceManager` internally to:
- Initialize all devices
- Start/stop devices
- Update devices periodically (1 second interval)

### Device Repository
The `SystemController` uses `DeviceRepository` to:
- Register all devices
- Provide centralized device access
- Support device queries and telemetry

### GUI Panels
Panels continue to subscribe directly to devices for real-time updates. The `SystemController` provides:
- Centralized device instantiation
- Additional event broadcasting layer (optional)
- Control methods for user actions
- System-wide constants access

## Benefits

1. **Centralized Management**: Single point of control for all devices
2. **Separation of Concerns**: GUI doesn't need to know about device creation
3. **Event Broadcasting**: Centralized event distribution to GUI components
4. **Automatic Actions**: Automatic device responses to sensor readings
5. **Logging**: Centralized logging of all system events
6. **Constants**: Single source of truth for system-wide constants
7. **Extensibility**: Easy to add new sensors, devices, or automatic actions
8. **Maintainability**: Clear separation between business logic and UI

## Testing

To verify the integration works:

1. Run the application - all devices should initialize
2. Watch console output for system events
3. Verify GUI panels update in real-time
4. Test user actions (turn on pump, set flow rate)
5. Observe automatic actions (chemical doser activates when pH out of range)
6. Check that alerts are logged and displayed

## File Structure

```
src/
├── SystemCore/
│   └── SystemController.cs       ← Integration layer
├── GUI/
│   ├── MainWindow.xaml.cs        ← Uses SystemController
│   └── Panels/
│       ├── PHMonitoringPanel.xaml.cs
│       ├── IntakePumpPanel.xaml.cs
│       └── FiltrationSensorPanel.xaml.cs
└── Devices/
    ├── Sensors/
    │   ├── pHSensor.cs
    │   └── FiltrationSensor.cs
    └── Devices/
        ├── IntakePump.cs
        └── ChemicalDoser.cs
```
