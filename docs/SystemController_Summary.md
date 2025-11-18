# SystemController Integration Layer - Summary

## Deliverables

### 1. File Paths

**Main Integration Layer:**
- `src/SystemCore/SystemController.cs` - Central system controller class

**Updated Files:**
- `src/GUI/MainWindow.xaml.cs` - Updated to use SystemController
- `src/SystemCore/SystemCore.csproj` - Added Devices project reference
- `src/GUI/GUI.csproj` - Fixed target framework to `net9.0-windows`

**Documentation:**
- `docs/SystemController_Integration.md` - Full integration documentation
- `docs/How_To_Run.md` - Running instructions and verification steps
- `docs/SystemController_Summary.md` - This summary document

### 2. Full Code

The complete `SystemController` class is located at:
**`src/SystemCore/SystemController.cs`**

Key components:

#### Global Constants (Lines 18-44)
```csharp
// pH monitoring constants
public const double LowerSafePH = 6.5;
public const double UpperSafePH = 8.5;
// ... and more
```

#### Device Instantiation (Lines 149-208)
- `InitializeSensors()` - Creates pH sensor and filtration sensor
- `InitializeDevices()` - Creates intake pump and chemical doser
- `LinkDevices()` - Links chemical doser to pH sensor

#### Event Subscription (Lines 230-265)
- Subscribes to all device events
- Routes events to internal handlers

#### Event Broadcasting (Lines 267-334)
- `OnPHReadingChanged` - Broadcasts pH readings
- `OnChemicalDoserStateChanged` - Broadcasts doser state
- `OnIntakePumpStateChanged` - Broadcasts pump state
- `OnIntakePumpFlowRateChanged` - Broadcasts flow rate
- `OnFiltrationTurbidityChanged` - Broadcasts turbidity
- `OnFiltrationAlert` - Broadcasts alerts
- `OnFiltrationAlertCleared` - Broadcasts alert clear

#### Automatic Actions (Lines 395-412)
- `HandlePHReading()` - Handles automatic responses to pH readings
- Currently: Chemical doser auto-activation (handled by device itself)
- Extensible: Can add more automatic actions here

#### Control Methods (Lines 415-474)
- `TurnOnIntakePump()` - User control: Turn pump on
- `TurnOffIntakePump()` - User control: Turn pump off
- `SetIntakePumpFlowRate()` - User control: Set flow rate
- `ActivateChemicalDoser()` - User control: Activate doser
- `DeactivateChemicalDoser()` - User control: Deactivate doser

#### Logging (Lines 551-591)
- `LogEvent()` - Logs all system events
- `OnSystemEvent` - Event for external logging systems
- `SystemEvent` class - Event data structure
- `SystemEventType` enum - Event type categories

### 3. Event Flow Explanation

#### Sensor Reading Flow

1. **DeviceManager** timer triggers every 1 second
2. **Device.Update()** reads from CSV simulation files
3. **Sensor** detects reading change (e.g., pH value changed)
4. **Sensor** raises event (e.g., `OnReadingChange`)
5. **SystemController** receives event in handler
6. **SystemController** logs event to console
7. **SystemController** triggers automatic actions if needed
8. **SystemController** broadcasts event to GUI (`OnPHReadingChanged`)
9. **GUI Panel** receives event (already subscribed to device directly)
10. **GUI Panel** updates UI via `Dispatcher.Invoke()`

#### Automatic Action Flow

1. **pHSensor** reading changes (e.g., pH = 6.2, out of range)
2. **pHSensor.OnReadingChange** event raised
3. **SystemController.OnPHSensorReadingChanged** receives event
4. **SystemController** logs warning about pH out of range
5. **ChemicalDoser** (already subscribed to pH sensor) receives event
6. **ChemicalDoser.OnPHSensorReadingChanged** handler checks pH
7. **ChemicalDoser.Activate()** called automatically (pH < 6.5)
8. **ChemicalDoser.OnStateChange** event raised
9. **SystemController.OnChemicalDoserStateChangedInternal** receives event
10. **SystemController** broadcasts to GUI (`OnChemicalDoserStateChanged`)
11. **GUI Panel** updates to show "ACTIVE" status

#### User Action Flow

1. **User** clicks "TURN ON" button in GUI
2. **IntakePumpPanel.PowerButton_Click** handler called
3. **IntakePump.TurnOn()** called directly (panel has device reference)
4. **IntakePump** updates state and raises `OnStateChange` event
5. **SystemController.OnIntakePumpStateChangedInternal** receives event
6. **SystemController** logs user action
7. **SystemController** broadcasts to GUI (`OnIntakePumpStateChanged`)
8. **IntakePumpPanel** (already subscribed) receives event
9. **IntakePumpPanel.UpdatePowerButton()** updates UI via `Dispatcher.Invoke()`

**Alternative (via SystemController):**
1. User action → `SystemController.TurnOnIntakePump()`
2. SystemController calls `IntakePump.TurnOn()`
3. [Same event flow as above]

### 4. Data Flow Diagram

```
┌─────────────────────────────────────────────────────────────────────────┐
│                         DATA FLOW ARCHITECTURE                           │
└─────────────────────────────────────────────────────────────────────────┘

┌──────────────┐
│  CSV Files   │ (Simulation Data)
│              │
│  • pH.csv    │
│  • Pump.csv  │
│  • Turb.csv  │
└──────┬───────┘
       │
       │ (Read every 1 second)
       ▼
┌─────────────────────────────────────────┐
│      DeviceManager (Timer)              │
│                                         │
│  • Updates devices every 1 second      │
│  • Calls device.Update()               │
└──────┬──────────────────────────────────┘
       │
       │ (Device.Update())
       ▼
┌─────────────────────────────────────────┐
│           SENSORS                       │
│                                         │
│  ┌──────────┐      ┌──────────┐       │
│  │pHSensor  │      │Filtration│       │
│  │          │      │Sensor    │       │
│  │Reading   │      │Turbidity │       │
│  │Changes   │      │Changes   │       │
│  └────┬─────┘      └────┬─────┘       │
│       │                 │             │
│       └────────┬────────┘             │
│                │                      │
│                ▼                      │
│       ┌────────────────┐             │
│       │ OnReadingChange│             │
│       │ OnTurbidity... │             │
│       └────┬───────────┘             │
└────────────┼──────────────────────────┘
             │
             │ (Events raised)
             ▼
┌─────────────────────────────────────────┐
│      SystemController                   │
│                                         │
│  ┌───────────────────────────────────┐ │
│  │ Event Handlers                    │ │
│  │  • OnPHSensorReadingChanged       │ │
│  │  • OnFiltrationTurbidityChanged   │ │
│  │  • OnChemicalDoserStateChanged    │ │
│  │  • OnIntakePumpStateChanged       │ │
│  └──────┬────────────────────────────┘ │
│         │                               │
│         ▼                               │
│  ┌───────────────────────────────────┐ │
│  │ Automatic Actions                 │ │
│  │  • HandlePHReading()              │ │
│  │  • (Chemical doser auto-activate) │ │
│  └──────┬────────────────────────────┘ │
│         │                               │
│         ▼                               │
│  ┌───────────────────────────────────┐ │
│  │ Logging                           │ │
│  │  • LogEvent()                     │ │
│  │  • OnSystemEvent                  │ │
│  └──────┬────────────────────────────┘ │
│         │                               │
│         ▼                               │
│  ┌───────────────────────────────────┐ │
│  │ Event Broadcasting                │ │
│  │  • OnPHReadingChanged             │ │
│  │  • OnChemicalDoserStateChanged    │ │
│  │  • OnIntakePumpStateChanged       │ │
│  │  • OnFiltrationTurbidityChanged   │ │
│  │  • OnFiltrationAlert              │ │
│  └──────┬────────────────────────────┘ │
└─────────┼───────────────────────────────┘
          │
          │ (Events broadcast)
          ▼
┌─────────────────────────────────────────┐
│           GUI PANELS                    │
│                                         │
│  ┌──────────────┐  ┌──────────────┐   │
│  │pH Monitoring │  │Intake Pump   │   │
│  │Panel         │  │Panel         │   │
│  │              │  │              │   │
│  │• pH Display  │  │• On/Off Btn  │   │
│  │• Doser Status│  │• Flow Slider │   │
│  │              │  │• Status Text │   │
│  └──────┬───────┘  └──────┬───────┘   │
│         │                 │            │
│         └────────┬────────┘            │
│                  │                     │
│                  ▼                     │
│       ┌──────────────────┐            │
│       │ Filtration Sensor│            │
│       │ Panel            │            │
│       │                  │            │
│       │• Turbidity Gauge │            │
│       │• Alert Banner    │            │
│       │• Status Info     │            │
│       └──────────────────┘            │
└─────────────────────────────────────────┘
          │
          │ (User Actions)
          ▼
┌─────────────────────────────────────────┐
│         USER ACTIONS                    │
│                                         │
│  • Click "TURN ON" button              │
│  • Move flow rate slider               │
│  • Observe real-time updates           │
└──────┬──────────────────────────────────┘
       │
       │ (Control methods called)
       ▼
┌─────────────────────────────────────────┐
│         DEVICES                         │
│                                         │
│  ┌──────────┐      ┌──────────┐       │
│  │IntakePump│      │Chemical  │       │
│  │          │      │Doser     │       │
│  │• TurnOn()│      │• Activate│       │
│  │• TurnOff │      │• Deact...│       │
│  │• SetFlow │      │• Monitor │       │
│  │  Rate()  │      │  pH      │       │
│  └────┬─────┘      └────┬─────┘       │
│       │                 │             │
│       └────────┬────────┘             │
│                │                      │
│                ▼                      │
│       ┌────────────────┐             │
│       │ OnStateChange  │             │
│       │ OnFlowRate...  │             │
│       └────┬───────────┘             │
└────────────┼──────────────────────────┘
             │
             │ (Events raised - loop back)
             ▼
    [Back to SystemController]
```

### Complete Event Loop

```
1. CSV Files → DeviceManager (Timer)
2. DeviceManager → Devices.Update()
3. Sensors → Detect Changes → Raise Events
4. SystemController → Subscribe → Handle → Log → Broadcast
5. GUI Panels → Receive Events → Update UI (Dispatcher.Invoke)
6. User Actions → GUI → Devices → Raise Events
7. [Loop back to step 4]
```

## How to Run

### Quick Start

```bash
# Option 1: Visual Studio
1. Open CSCN72020-Sec3-Group3.sln
2. Set GUI as startup project
3. Press F5

# Option 2: Command Line
cd src/GUI
dotnet run
```

### Verification Steps

1. **Check Console Output**
   - System initialization messages
   - Real-time sensor updates
   - Event logging

2. **Verify Real-Time Updates**
   - pH values update every 1 second
   - Turbidity values update every 1 second
   - Flow rate updates when pump is on

3. **Test User Actions**
   - Turn intake pump on/off
   - Adjust flow rate slider
   - Observe UI updates

4. **Test Automatic Actions**
   - Watch pH go out of range → Chemical doser activates
   - Watch turbidity exceed threshold → Alert banner appears

5. **Monitor Event Logging**
   - Console shows all system events
   - Events categorized (Info, Warning, Alert, etc.)

See `docs/How_To_Run.md` for detailed instructions.

## Key Features

✅ **Centralized Device Management** - Single point for all device instantiation  
✅ **Event-Driven Architecture** - Real-time updates via events  
✅ **Automatic Actions** - Devices respond automatically to sensor readings  
✅ **Event Broadcasting** - Centralized event distribution to GUI  
✅ **Logging** - Comprehensive event logging with categories  
✅ **Global Constants** - Single source of truth for system-wide values  
✅ **User Control Methods** - Interface for user actions  
✅ **Thread Safety** - Proper Dispatcher.Invoke for UI updates  
✅ **Extensibility** - Easy to add new sensors, devices, or actions  
✅ **Maintainability** - Clear separation of concerns  

## Architecture Benefits

1. **Separation of Concerns**: GUI doesn't need to know about device creation
2. **Centralized Control**: All device management in one place
3. **Event Broadcasting**: Multiple subscribers can receive updates
4. **Logging**: Centralized logging for debugging and monitoring
5. **Constants**: Single source of truth for thresholds and limits
6. **Extensibility**: Easy to add new automatic actions or devices
7. **Maintainability**: Clear structure makes changes easy

## Next Steps

- Add more automatic actions in `HandlePHReading()`
- Implement file-based logging (subscribe to `OnSystemEvent`)
- Add log viewer UI component
- Extend automatic actions for more device interactions
- Add configuration file support for constants
- Implement data persistence for historical data
