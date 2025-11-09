# Water Treatment Plant SCADA/HMI System

**Course:** CSCN72030 F25v2  
**Team:** Michael Campbell, Navneet Chatha, Jason Little, Mario Ezeh

## Project Description

A SCADA/HMI system for monitoring and controlling operations in a sewage water treatment plant using Repository pattern architecture.

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
- **DeviceManager** - Central coordination
- **DeviceRepository** - Device storage and retrieval
- **FileSimulator/BinaryIO** - Data simulation
- **DeviceCommand** - Command pattern implementation

## Technology Stack

- **Language:** C# (.NET 6.0+)
- **GUI:** WPF (Windows Presentation Foundation)
- **Architecture:** Repository Pattern with SOLID principles
- **Data Simulation:** CSV/Binary file-based
- **IDE:** Visual Studio 2022

## Project Structure

```
WaterTreatmentPlant-SCADA/
├── docs/                       # Documentation
├── src/
│   ├── Core/                   # Core interfaces and base classes
│   │   ├── Interfaces/         # IDevice, IDeviceRepository, IFileSimulator
│   │   ├── Models/             # DeviceStatus, DeviceCommand
│   │   ├── Base/               # BaseDevice, ControllableDevice
│   │   └── Utilities/          # BinaryIO
│   ├── Devices/                # Device implementations (9 devices)
│   ├── SystemCore/             # DeviceManager, DeviceRepository
│   ├── GUI/                    # WPF application
│   └── TestConsole/            # Console test app
├── data/
│   ├── simulations/            # CSV simulation files
│   └── logs/                   # Runtime logs
└── tests/                      # Unit and integration tests
```

## Team Responsibilities

**Member 1:** WaterInletDevice, WaterStorageSensor, FiltrationUnit  
**Member 2:** CoagulationSensor, PressureChangeDevice, ChlorinePumpDevice  
**Member 3:** PHChangeDevice, TemperatureDevice, UVSterilizer  
**Member 4:** DeviceManager, DeviceRepository, GUI, Integration

## Build Instructions

### Prerequisites
- Visual Studio 2022 or later
- .NET 6.0 SDK or later
- Git
- Windows 10/11

### Setup
```bash
# Clone repository
git clone https://github.com/[your-username]/WaterTreatmentPlant-SCADA.git
cd WaterTreatmentPlant-SCADA

# Open solution
cd src
# Open WaterTreatmentSCADA.sln in Visual Studio

# Build
dotnet build

# Run test console
dotnet run --project TestConsole
```

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

## Resources

- [Implementation Guide](docs/updated_implementation_guide.md)
- [Class Diagram](docs/design/ClassDiagram.png)
- [Requirements](docs/requirements/)

---

**Last Updated:** October 30, 2025
