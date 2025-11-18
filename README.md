# Water Treatment Plant SCADA/HMI System

**Course:** CSCN72030 F25v2  
**Team:** Michael Campbell, Navneet Chatha, Jason Little, Mario Ezeh

## Project Description

A SCADA/HMI system for monitoring and controlling operations in a sewage water treatment plant using Repository pattern architecture. Sprint 2 completed with full GUI integration, real-time monitoring, and automatic device control.

## Architecture

### Design Pattern
- **Repository Pattern** for device management
- **Inheritance Hierarchy** with BaseDevice and ControllableDevice
- **Interface-based** design for extensibility

### Class Structure
```
IDeviceRepository ←→ DeviceManager
        ↓
    IDevice
        ↓
   BaseDevice (abstract)
        ↓
   ├── ControllableDevice (abstract)
   │   ├── WaterInletDevice
   │   ├── ChlorinePumpDevice
   │   ├── PressureChangeDevice
   │   ├── PHChangeDevice
   │   ├── TemperatureDevice
   │   ├── FiltrationUnit
   │   └── UVSterilizer
   │
   ├── CoagulationSensor
   └── WaterStorageSensor
```

## System Components

### Controllable Devices (7)
1. **WaterInletDevice** - Raw water intake control
2. **ChlorinePumpDevice** - Chemical dosing control
3. **PressureChangeDevice** - Pressure regulation
4. **PHChangeDevice** - pH level adjustment
5. **TemperatureDevice** - Temperature control
6. **FiltrationUnit** - Filtration process control
7. **UVSterilizer** - UV disinfection control

### Sensors (2)
8. **CoagulationSensor** - Coagulation monitoring
9. **WaterStorageSensor** - Storage level monitoring

### Core System
- **SystemController** - Central integration layer for sensors, devices, and GUI
- **DeviceManager** - Central coordination and periodic updates
- **DeviceRepository** - Device storage and retrieval
- **FileSimulator/BinaryIO** - Data simulation

### Sprint 2 Components (Implemented)
- **pHSensor** - pH level monitoring (5.0 - 9.0 range)
- **FiltrationSensor** - Turbidity monitoring in NTU (0-10 range)
- **IntakePump** - Water flow control with variable flow rate (0-100%)
- **ChemicalDoser** - Automatic pH correction device
- **GUI Panels** - Real-time monitoring and control panels
  - pH Monitoring Panel with automatic doser activation
  - Intake Pump Control Panel with flow rate adjustment
  - Filtration Sensor Panel with turbidity alerts

## Technology Stack

- **Language:** C# (.NET 9.0)
- **GUI:** WPF (Windows Presentation Foundation)
- **Architecture:** Repository Pattern with SOLID principles
- **Data Simulation:** CSV/Binary file-based
- **IDE:** Visual Studio 2022 or later

## Project Structure

```
WaterTreatmentPlant-SCADA/
├── docs/                       # Documentation
│   ├── SystemController_Integration.md  # Integration layer docs
│   ├── SystemController_Summary.md      # Quick reference
│   └── How_To_Run.md                   # Running instructions
├── src/
│   ├── Core/                   # Core interfaces and base classes
│   │   ├── Interfaces/         # IDevice, IDeviceRepository, IFileSimulator
│   │   ├── Models/             # DeviceStatus, DeviceCommand
│   │   ├── Base/               # BaseDevice, ControllableDevice
│   │   └── Utilities/          # BinaryIO
│   ├── Devices/                # Device implementations
│   │   ├── Sensors/            # pHSensor, FiltrationSensor
│   │   └── Devices/            # IntakePump, ChemicalDoser
│   ├── SystemCore/             # SystemController, DeviceManager, DeviceRepository
│   ├── GUI/                    # WPF application with panels
│   └── TestConsole/            # Console test app
├── data/
│   └── simulations/            # CSV simulation files
└── tests/                      # Unit and integration tests
```

## Team Responsibilities

**Member 1:** WaterInletDevice, WaterStorageSensor, FiltrationUnit  
**Member 2:** CoagulationSensor, PressureChangeDevice, ChlorinePumpDevice  
**Member 3:** PHChangeDevice, TemperatureDevice, UVSterilizer  
**Member 4:** DeviceManager, DeviceRepository, GUI, Integration

## Sprint 2 Features

### User Story 1: pH Monitoring + Automatic Chemical Dosing
- Real-time pH readings updated every 1 second
- Automatic chemical doser activation when pH < 6.5 or pH > 8.5
- Color-coded GUI display (blue = safe, red = out of range)
- Live doser status indicator (ACTIVE/INACTIVE)

### User Story 2: Intake Pump Control
- ON/OFF pump control via button
- Flow rate adjustment slider (0-100%)
- Real-time flow rate display
- Automatic status updates

### User Story 3: Filtration Turbidity Monitoring
- Real-time turbidity readings (NTU) updated every 1 second
- Color-coded safety indicators (green/yellow/red)
- Alert banner when turbidity > 5 NTU
- Threshold status display

### System-Wide Features
- **SystemController** - Central integration layer for all devices and GUI
- Event-driven architecture with real-time updates
- Automatic device actions based on sensor readings
- Comprehensive event logging

## Build Instructions

### Prerequisites
- Visual Studio 2022 or later
- .NET 9.0 SDK
- Git
- Windows 10/11 (WPF requires Windows)

### Setup
```bash
# Clone repository
git clone https://github.com/[your-username]/WaterTreatmentPlant-SCADA.git
cd WaterTreatmentPlant-SCADA

# Open solution in Visual Studio
# File -> Open -> CSCN72020-Sec3-Group3.sln

# Or build from command line
dotnet build CSCN72020-Sec3-Group3.sln

# Run GUI application
cd src/GUI
dotnet run
```

### Running the Application

1. **From Visual Studio:**
   - Open `CSCN72020-Sec3-Group3.sln`
   - Set `GUI` project as startup project
   - Press F5 to build and run

2. **From Command Line:**
   ```bash
   cd src/GUI
   dotnet run
   ```

**Important:** Ensure simulation CSV files exist in `data/simulations/`:
- `pHSensor_simulation.csv`
- `IntakePump_simulation.csv`
- `TurbiditySensor_simulation.csv`

## Development Workflow

### Daily Standup
- Time: [Set by team]
- Format: Completed / Doing / Blockers

### Git Workflow
```bash
git pull origin main
git checkout -b feature/device-name
# ... make changes ...
git add .
git commit -m "Implement DeviceName"
git push origin feature/device-name
# Create Pull Request
```

## Key Milestones

| Date | Milestone |
|------|-----------|
| Nov 1 | All 9 devices implemented |
| Nov 8 | Full integration with GUI |
| Nov 27 | Usability testing |
| Dec 12 | Final delivery |

## Architecture (Sprint 2)

### SystemController Integration Layer
The `SystemController` class provides centralized management of all sensors, devices, and GUI components:
- Instantiates all sensors and devices
- Subscribes to all device events
- Broadcasts updates to GUI components
- Contains system-wide constants (pH ranges, turbidity limits, etc.)
- Handles automatic device actions (e.g., doser activation based on pH)
- Provides event logging for debugging

### Event Flow
```
CSV Files → DeviceManager (Timer) → Sensors → SystemController → GUI → User Actions → Devices
                                                                        ↓
                                                                   Automatic
                                                                   Actions
```

### Real-Time Updates
- All sensors update every 1 second via DeviceManager timer
- Events are fired when readings change
- GUI panels update automatically using Dispatcher.Invoke()
- No UI freezing - updates happen on separate thread

See [SystemController Integration Documentation](docs/SystemController_Integration.md) for detailed architecture.

## Resources

- [SystemController Integration Guide](docs/SystemController_Integration.md) - Full integration documentation
- [SystemController Summary](docs/SystemController_Summary.md) - Quick reference with diagrams
- [How to Run](docs/How_To_Run.md) - Running instructions and verification steps
- [GUI README](src/GUI/README.md) - GUI component details
- [Requirements](docs/requirements/) - Project requirements
- [Design Documentation](docs/design/) - Design documents

---

**Last Updated:** January 2025 (Sprint 2 Complete)
