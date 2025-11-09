#!/bin/bash

# Water Treatment Plant SCADA/HMI - Updated Setup Script
# Based on actual class diagram architecture
# This script creates the project structure matching your design

echo "🚀 Water Treatment Plant SCADA/HMI - Updated Project Setup"
echo "         (Based on Your Class Diagram)"
echo "============================================================"
echo ""

# Check if we're in a git repository
if [ ! -d ".git" ]; then
    echo "❌ Error: Not in a git repository!"
    echo "Please run this script in your cloned repository directory."
    exit 1
fi

echo "✅ Git repository detected"
echo ""

# Create directory structure matching class diagram
echo "📁 Creating directory structure..."

mkdir -p docs/requirements
mkdir -p docs/design
mkdir -p docs/testing

mkdir -p src/Core/Interfaces
mkdir -p src/Core/Models
mkdir -p src/Core/Base
mkdir -p src/Core/Utilities

mkdir -p src/Devices/WaterInletDevice
mkdir -p src/Devices/ChlorinePumpDevice
mkdir -p src/Devices/PressureChangeDevice
mkdir -p src/Devices/PHChangeDevice
mkdir -p src/Devices/TemperatureDevice
mkdir -p src/Devices/CoagulationSensor
mkdir -p src/Devices/WaterStorageSensor
mkdir -p src/Devices/FiltrationUnit
mkdir -p src/Devices/UVSterilizer

mkdir -p src/SystemCore
mkdir -p src/GUI/WaterTreatmentHMI
mkdir -p src/TestConsole

mkdir -p data/simulations
mkdir -p data/logs

mkdir -p tests/UnitTests
mkdir -p tests/IntegrationTests

echo "✅ Directory structure created"
echo ""

# Create .gitignore
echo "📝 Creating .gitignore..."
cat > .gitignore << 'EOF'
# Build results
[Dd]ebug/
[Dd]ebugPublic/
[Rr]elease/
[Rr]eleases/
x64/
x86/
[Ww][Ii][Nn]32/
[Aa][Rr][Mm]/
[Aa][Rr][Mm]64/
bld/
[Bb]in/
[Oo]bj/
[Ll]og/
[Ll]ogs/

# Visual Studio
.vs/
*.suo
*.user
*.userosscache
*.sln.docstates
*.userprefs

# Test Results
[Tt]est[Rr]esult*/
[Bb]uild[Ll]og.*

# NuGet
*.nupkg
*.snupkg
**/packages/*
*.nuget.props
*.nuget.targets

# Runtime logs
data/logs/*.log
data/logs/*.txt
*.log

# OS Files
.DS_Store
Thumbs.db
*.swp
*~

# IDE
.vscode/
.idea/
*.sublime-*
EOF
echo "✅ .gitignore created"
echo ""

# Create comprehensive README
echo "📝 Creating README.md..."
cat > README.md << 'EOF'
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
EOF
echo "✅ README.md created"
echo ""

# Create Core Interface files
echo "📝 Creating core interface files..."

cat > src/Core/Interfaces/IDevice.cs << 'EOF'
using System;
using System.Collections.Generic;

namespace WaterTreatmentSCADA.Core.Interfaces
{
    public interface IDevice
    {
        string Name { get; }
        string DeviceType { get; }
        DeviceStatus Status { get; }
        DateTime LastUpdate { get; }
        
        void Initialize();
        void Start();
        void Stop();
        void Update();
        Dictionary<string, object> GetTelemetryData();
    }
    
    public enum DeviceStatus
    {
        Offline,
        Online,
        Warning,
        Critical,
        Error,
        Maintenance
    }
}
EOF

cat > src/Core/Interfaces/IDeviceRepository.cs << 'EOF'
using System.Collections.Generic;

namespace WaterTreatmentSCADA.Core.Interfaces
{
    public interface IDeviceRepository
    {
        List<IDevice> Devices { get; }
        IDevice GetDevice(string deviceName);
        void AddDevice(IDevice device);
        void RemoveDevice(string deviceName);
        List<IDevice> GetAllDevices();
    }
}
EOF

cat > src/Core/Interfaces/IFileSimulator.cs << 'EOF'
namespace WaterTreatmentSCADA.Core.Interfaces
{
    public interface IFileSimulator
    {
        string FilePath { get; }
        string ReadLine();
        void WriteLine(string data);
        void Reset();
        bool IsEndOfFile { get; }
    }
}
EOF

echo "✅ Core interface files created"
echo ""

# Create simulation data files
echo "📝 Creating simulation data files..."

cat > data/simulations/WaterInletDevice_simulation.csv << 'EOF'
timestamp,flowRate,isRunning,pressure,status
0,45.5,true,2.3,normal
1,46.2,true,2.4,normal
2,47.1,true,2.5,normal
3,45.8,true,2.3,normal
4,44.2,true,2.2,normal
5,43.5,true,2.1,warning
6,42.1,true,2.0,warning
7,39.0,true,1.9,warning
8,28.5,true,1.7,critical
9,35.0,true,1.8,warning
10,42.0,true,2.0,warning
11,45.0,true,2.3,normal
12,46.5,true,2.4,normal
13,47.2,true,2.5,normal
14,45.9,true,2.3,normal
EOF

cat > data/simulations/CoagulationSensor_simulation.csv << 'EOF'
timestamp,coagulationLevel,status
0,92.5,normal
1,91.8,normal
2,90.2,normal
3,88.5,normal
4,85.0,normal
5,82.3,warning
6,78.5,warning
7,75.2,warning
8,58.5,critical
9,65.0,warning
10,72.5,warning
11,80.0,warning
12,86.5,normal
13,91.0,normal
14,93.5,normal
EOF

cat > data/simulations/ChlorinePumpDevice_simulation.csv << 'EOF'
timestamp,chlorineLevel,dosingRate,isRunning,status
0,2.1,0.5,true,normal
1,2.2,0.5,true,normal
2,2.3,0.5,true,normal
3,2.2,0.5,true,normal
4,1.8,0.7,true,warning
5,1.6,0.9,true,warning
6,1.4,1.0,true,critical
7,1.7,0.8,true,warning
8,2.0,0.6,true,normal
9,2.1,0.5,true,normal
10,2.2,0.5,true,normal
EOF

cat > data/simulations/PressureChangeDevice_simulation.csv << 'EOF'
timestamp,pressure,targetPressure,isRunning,status
0,2.3,2.5,true,normal
1,2.4,2.5,true,normal
2,2.5,2.5,true,normal
3,2.8,2.5,true,warning
4,3.1,2.5,true,critical
5,2.9,2.5,true,warning
6,2.6,2.5,true,normal
7,2.4,2.5,true,normal
8,2.3,2.5,true,normal
9,2.5,2.5,true,normal
EOF

cat > data/simulations/PHChangeDevice_simulation.csv << 'EOF'
timestamp,phValue,targetPH,isAdjusting,status
0,7.2,7.0,false,normal
1,7.3,7.0,false,normal
2,7.1,7.0,false,normal
3,8.6,7.0,true,warning
4,8.9,7.0,true,critical
5,8.2,7.0,true,warning
6,7.5,7.0,true,normal
7,7.2,7.0,false,normal
8,7.0,7.0,false,normal
9,7.1,7.0,false,normal
EOF

cat > data/simulations/TemperatureDevice_simulation.csv << 'EOF'
timestamp,temperature,targetTemp,isHeating,status
0,18.5,20.0,true,normal
1,19.2,20.0,true,normal
2,20.1,20.0,false,normal
3,21.5,20.0,false,warning
4,22.8,20.0,false,warning
5,21.2,20.0,false,warning
6,20.0,20.0,false,normal
7,19.5,20.0,true,normal
8,18.9,20.0,true,normal
9,20.0,20.0,false,normal
EOF

cat > data/simulations/WaterStorageSensor_simulation.csv << 'EOF'
timestamp,storageAmount,capacity,percentFull,status
0,7500,10000,75.0,normal
1,7350,10000,73.5,normal
2,7200,10000,72.0,normal
3,7000,10000,70.0,normal
4,6500,10000,65.0,warning
5,6000,10000,60.0,warning
6,5500,10000,55.0,warning
7,5000,10000,50.0,critical
8,5500,10000,55.0,warning
9,6200,10000,62.0,warning
10,7000,10000,70.0,normal
EOF

cat > data/simulations/FiltrationUnit_simulation.csv << 'EOF'
timestamp,filterPressure,flowRate,efficiency,isRunning,status
0,1.2,85.5,94.2,true,normal
1,1.3,86.1,94.5,true,normal
2,1.4,84.8,93.8,true,normal
3,1.5,83.2,92.5,true,warning
4,1.6,82.1,91.2,true,warning
5,1.7,81.0,90.0,true,warning
6,1.8,80.2,89.2,true,critical
7,1.5,83.5,92.5,true,warning
8,1.3,85.0,94.0,true,normal
9,1.2,86.0,94.5,true,normal
EOF

cat > data/simulations/UVSterilizer_simulation.csv << 'EOF'
timestamp,uvIntensity,lampStatus,isActive,status
0,85.2,good,true,normal
1,86.1,good,true,normal
2,85.5,good,true,normal
3,82.3,degraded,true,warning
4,78.1,degraded,true,warning
5,72.5,poor,true,critical
6,75.0,degraded,true,warning
7,80.0,degraded,true,warning
8,84.0,good,true,normal
9,85.8,good,true,normal
EOF

echo "✅ Simulation data files created (9 devices)"
echo ""

# Create documentation files
cat > docs/requirements/README.md << 'EOF'
# Requirements Documentation

Store your requirements documents here:
- System requirements
- Functional requirements
- Non-functional requirements
- Interface requirements
EOF

cat > docs/design/README.md << 'EOF'
# Design Documentation

Store your design documents here:
- Class diagrams
- Sequence diagrams
- Architecture diagrams
- UI/UX mockups

**Note:** Add your ClassDiagram.png here!
EOF

cat > docs/testing/README.md << 'EOF'
# Testing Documentation

Store your testing documents here:
- Test plans
- Test cases
- Usability test results
- Bug reports
EOF

cat > data/simulations/README.md << 'EOF'
# Simulation Data Files

This directory contains CSV simulation files for all 9 devices.

## File Format
CSV format with headers. Files loop when reaching end.

## Device Files
1. WaterInletDevice_simulation.csv
2. ChlorinePumpDevice_simulation.csv
3. PressureChangeDevice_simulation.csv
4. PHChangeDevice_simulation.csv
5. TemperatureDevice_simulation.csv
6. CoagulationSensor_simulation.csv
7. WaterStorageSensor_simulation.csv
8. FiltrationUnit_simulation.csv
9. UVSterilizer_simulation.csv
EOF

echo "✅ Documentation structure created"
echo ""

# Summary
echo ""
echo "============================================================"
echo "✅ Updated project setup complete!"
echo "============================================================"
echo ""
echo "📁 Created:"
echo "   - 28 directories (matching class diagram)"
echo "   - 9 simulation CSV files (all devices)"
echo "   - 3 core interface files"
echo "   - Comprehensive README"
echo "   - Documentation structure"
echo ""
echo "🎯 Device Structure:"
echo "   Controllable Devices (7):"
echo "   - WaterInletDevice"
echo "   - ChlorinePumpDevice"
echo "   - PressureChangeDevice"
echo "   - PHChangeDevice"
echo "   - TemperatureDevice"
echo "   - FiltrationUnit"
echo "   - UVSterilizer"
echo ""
echo "   Sensors (2):"
echo "   - CoagulationSensor"
echo "   - WaterStorageSensor"
echo ""
echo "📋 Next steps:"
echo "   1. Review generated files"
echo "   2. Copy your ClassDiagram.png to docs/design/"
echo "   3. Commit to git:"
echo "      git add ."
echo "      git commit -m 'Initial project structure (class diagram architecture)'"
echo "      git push origin main"
echo "   4. Team members pull changes"
echo "   5. Start device implementation using updated_implementation_guide.md"
echo ""
echo "🚀 Ready to implement!"
echo ""
