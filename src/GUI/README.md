# GUI Components - Sprint 2

## Overview

This GUI application provides real-time monitoring and control for the Water Treatment Plant SCADA system. It consists of three main panels that subscribe to device events and update in real-time.

## Project Structure

```
src/GUI/
├── GUI.csproj              # WPF project file
├── App.xaml                # Application entry point
├── App.xaml.cs             # Application code-behind
├── MainWindow.xaml         # Main window layout
├── MainWindow.xaml.cs      # Main window logic
└── Panels/
    ├── PHMonitoringPanel.xaml
    ├── PHMonitoringPanel.xaml.cs
    ├── IntakePumpPanel.xaml
    ├── IntakePumpPanel.xaml.cs
    ├── FiltrationSensorPanel.xaml
    └── FiltrationSensorPanel.xaml.cs
```

## Components

### 1. pH Monitoring Panel

**File:** `Panels/PHMonitoringPanel.xaml` / `PHMonitoringPanel.xaml.cs`

**Features:**
- Live-updating pH value display (large, prominent)
- Color indicator (blue when safe, red when out of range)
- Safe range indicator (green dot when in range 6.5-8.5)
- Chemical Doser status indicator (ACTIVE/INACTIVE)

**Event Subscriptions:**
- `pHSensor.OnReadingChange` - Updates pH display and color
- `ChemicalDoser.OnStateChange` - Updates doser status

**Usage:**
```csharp
var phSensor = new pHSensor("Main pH Sensor", "path/to/simulation.csv");
var doser = new ChemicalDoser("Chemical Doser");
doser.SetPHSensor(phSensor);

var panel = new PHMonitoringPanel();
panel.Initialize(phSensor, doser);
```

### 2. Intake Pump Panel

**File:** `Panels/IntakePumpPanel.xaml` / `IntakePumpPanel.xaml.cs`

**Features:**
- ON/OFF toggle button
- Flow rate slider (0-100%)
- Real-time flow rate display
- Status information

**Event Subscriptions:**
- `IntakePump.OnStateChange` - Updates button and enables/disables slider
- `IntakePump.OnFlowRateChange` - Updates flow rate display and slider

**Usage:**
```csharp
var pump = new IntakePump("Main Intake Pump", "path/to/simulation.csv");

var panel = new IntakePumpPanel();
panel.Initialize(pump);
```

### 3. Filtration Sensor Panel

**File:** `Panels/FiltrationSensorPanel.xaml` / `FiltrationSensorPanel.xaml.cs`

**Features:**
- Live turbidity gauge with color coding:
  - Green: < 3 NTU
  - Yellow: 3-5 NTU
  - Red: > 5 NTU
- Alert banner for high turbidity (>5 NTU)
- Threshold status indicators
- Sensor information display

**Event Subscriptions:**
- `FiltrationSensor.OnTurbidityChange` - Updates gauge and color
- `FiltrationSensor.OnThresholdAlert` - Shows alert banner
- `FiltrationSensor.OnThresholdCleared` - Hides alert banner

**Usage:**
```csharp
var sensor = new FiltrationSensor("Filtration Sensor", "path/to/simulation.csv");

var panel = new FiltrationSensorPanel();
panel.Initialize(sensor);
```

## Running the Application

### Prerequisites
- .NET 9.0 SDK
- Visual Studio 2022 or later (for WPF support)
- Windows OS (WPF is Windows-only)

### Build and Run

1. **From Visual Studio:**
   - Open `CSCN72020-Sec3-Group3.sln`
   - Set `GUI` project as startup project
   - Press F5 to build and run

2. **From Command Line:**
   ```bash
   cd src/GUI
   dotnet build
   dotnet run
   ```

### Important Notes

- The application expects simulation CSV files in `data/simulations/` directory
- Ensure the following files exist:
  - `pHSensor_simulation.csv`
  - `IntakePump_simulation.csv`
  - `TurbiditySensor_simulation.csv`
- The `MainWindow` automatically initializes all devices and subscribes to events
- Devices are started automatically when the window loads
- Devices are stopped when the window closes

## Architecture

### Event-Driven Updates

All panels use event-driven architecture:
- Devices emit events when values change
- Panels subscribe to these events in their `Initialize()` method
- UI updates are dispatched to the UI thread using `Dispatcher.Invoke()`

### Thread Safety

All event handlers use `Dispatcher.Invoke()` to ensure UI updates happen on the UI thread, preventing cross-thread exceptions.

### Clean Component Architecture

- Each panel is a self-contained `UserControl`
- Panels expose an `Initialize()` method that takes device references
- Panels handle their own event subscriptions
- Main window only needs to create devices and call `Initialize()`

## Integration Example

```csharp
// In MainWindow.xaml.cs
private void InitializeDevices()
{
    // Create devices
    var phSensor = new pHSensor("Main pH Sensor", phPath);
    var doser = new ChemicalDoser("Chemical Doser");
    doser.SetPHSensor(phSensor);
    var pump = new IntakePump("Main Intake Pump", pumpPath);
    var filterSensor = new FiltrationSensor("Filtration Sensor", filterPath);
    
    // Initialize panels
    PhMonitoringPanel.Initialize(phSensor, doser);
    IntakePumpPanel.Initialize(pump);
    FiltrationSensorPanel.Initialize(filterSensor);
    
    // Start device manager
    var repository = new DeviceRepository();
    repository.AddDevice(phSensor);
    repository.AddDevice(doser);
    repository.AddDevice(pump);
    repository.AddDevice(filterSensor);
    
    var manager = new DeviceManager(repository);
    manager.InitializeAllDevices();
    manager.StartAllDevices();
}
```

## Troubleshooting

### "File not found" errors
- Ensure simulation CSV files exist in `data/simulations/`
- Check file paths in `MainWindow.xaml.cs`

### UI not updating
- Verify devices are started (`deviceManager.StartAllDevices()`)
- Check that event subscriptions are set up in `Initialize()` methods
- Ensure simulation files contain valid data

### Build errors
- Ensure all project references are correct in `GUI.csproj`
- Verify solution file includes all projects
- Check that .NET 9.0 SDK is installed

