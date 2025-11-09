#!/bin/bash

echo "Creating missing core files..."

# 1. BaseDevice.cs
cat > src/Core/Base/BaseDevice.cs << 'EOF'
using System;
using System.Collections.Generic;
using WaterTreatmentSCADA.Core.Interfaces;

namespace WaterTreatmentSCADA.Core.Base
{
    /// <summary>
    /// Abstract base class for all devices
    /// </summary>
    public abstract class BaseDevice : IDevice
    {
        public string Name { get; protected set; }
        public string DeviceType { get; protected set; }
        public DeviceStatus Status { get; protected set; }
        public DateTime LastUpdate { get; protected set; }
        
        protected IFileSimulator fileSimulator;
        protected bool isRunning;
        
        protected BaseDevice(string name, string deviceType, string simulationFilePath)
        {
            Name = name;
            DeviceType = deviceType;
            Status = DeviceStatus.Offline;
            isRunning = false;
            
            if (!string.IsNullOrEmpty(simulationFilePath))
            {
                fileSimulator = new BinaryIO(simulationFilePath);
            }
        }
        
        protected string GetFileInfo()
        {
            if (fileSimulator != null && !fileSimulator.IsEndOfFile)
            {
                return fileSimulator.ReadLine();
            }
            else if (fileSimulator != null)
            {
                fileSimulator.Reset();
                return fileSimulator.ReadLine();
            }
            return null;
        }
        
        public virtual void Initialize()
        {
            Status = DeviceStatus.Online;
            LastUpdate = DateTime.Now;
        }
        
        public virtual void Start()
        {
            isRunning = true;
            Status = DeviceStatus.Online;
        }
        
        public virtual void Stop()
        {
            isRunning = false;
            Status = DeviceStatus.Offline;
        }
        
        public abstract void Update();
        public abstract Dictionary<string, object> GetTelemetryData();
    }
}
EOF

# 2. ControllableDevice.cs
cat > src/Core/Base/ControllableDevice.cs << 'EOF'
using System;

namespace WaterTreatmentSCADA.Core.Base
{
    /// <summary>
    /// Base class for devices that can be controlled by operators
    /// </summary>
    public abstract class ControllableDevice : BaseDevice
    {
        public bool IsRunning => isRunning;
        
        protected ControllableDevice(string name, string deviceType, string simulationFilePath)
            : base(name, deviceType, simulationFilePath)
        {
        }
        
        public abstract void SetConfig(string configName, object value);
        public abstract object GetConfig(string configName);
        
        public virtual void TurnOn()
        {
            isRunning = true;
            Status = DeviceStatus.Online;
            Console.WriteLine($"{Name} turned ON");
        }
        
        public virtual void TurnOff()
        {
            isRunning = false;
            Status = DeviceStatus.Offline;
            Console.WriteLine($"{Name} turned OFF");
        }
    }
}
EOF

# 3. BinaryIO.cs
cat > src/Core/Utilities/BinaryIO.cs << 'EOF'
using System;
using System.IO;
using WaterTreatmentSCADA.Core.Interfaces;

namespace WaterTreatmentSCADA.Core.Utilities
{
    /// <summary>
    /// Binary file I/O implementation for device simulation
    /// </summary>
    public class BinaryIO : IFileSimulator
    {
        public string FilePath { get; private set; }
        public bool IsEndOfFile { get; private set; }
        
        private FileStream fileStream;
        private StreamReader reader;
        
        public BinaryIO(string filePath)
        {
            FilePath = filePath;
            IsEndOfFile = false;
            
            if (File.Exists(filePath))
            {
                fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read);
                reader = new StreamReader(fileStream);
                
                // Skip header line if CSV format
                if (filePath.EndsWith(".csv"))
                {
                    reader.ReadLine();
                }
            }
            else
            {
                throw new FileNotFoundException($"Simulation file not found: {filePath}");
            }
        }
        
        public string ReadLine()
        {
            if (reader != null && !reader.EndOfStream)
            {
                string line = reader.ReadLine();
                IsEndOfFile = reader.EndOfStream;
                return line;
            }
            
            IsEndOfFile = true;
            return null;
        }
        
        public void WriteLine(string data)
        {
            using (var logWriter = new StreamWriter(FilePath, true))
            {
                logWriter.WriteLine(data);
            }
        }
        
        public void Reset()
        {
            if (reader != null)
            {
                reader.BaseStream.Seek(0, SeekOrigin.Begin);
                reader.DiscardBufferedData();
                
                if (FilePath.EndsWith(".csv"))
                {
                    reader.ReadLine();
                }
                
                IsEndOfFile = false;
            }
        }
        
        ~BinaryIO()
        {
            reader?.Dispose();
            fileStream?.Dispose();
        }
    }
}
EOF

# 4. DeviceCommand.cs
cat > src/Core/Models/DeviceCommand.cs << 'EOF'
using System;

namespace WaterTreatmentSCADA.Core.Models
{
    /// <summary>
    /// Represents a command to be executed on a device
    /// </summary>
    public class DeviceCommand
    {
        public string DeviceName { get; set; }
        public string Command { get; set; }
        public object[] Parameters { get; set; }
        public DateTime Timestamp { get; set; }
        
        public DeviceCommand(string deviceName, string command, params object[] parameters)
        {
            DeviceName = deviceName;
            Command = command;
            Parameters = parameters;
            Timestamp = DateTime.Now;
        }
        
        public override string ToString()
        {
            return $"[{Timestamp:HH:mm:ss}] {DeviceName}: {Command}";
        }
    }
}
EOF

# 5. DeviceRepository.cs
cat > src/SystemCore/DeviceRepository.cs << 'EOF'
using System;
using System.Collections.Generic;
using System.Linq;
using WaterTreatmentSCADA.Core.Interfaces;

namespace WaterTreatmentSCADA.SystemCore
{
    /// <summary>
    /// Repository for managing all devices in the system
    /// </summary>
    public class DeviceRepository : IDeviceRepository
    {
        public List<IDevice> Devices { get; private set; }
        
        public DeviceRepository()
        {
            Devices = new List<IDevice>();
        }
        
        public IDevice GetDevice(string deviceName)
        {
            return Devices.FirstOrDefault(d => d.Name.Equals(deviceName, StringComparison.OrdinalIgnoreCase));
        }
        
        public void AddDevice(IDevice device)
        {
            if (device == null)
                throw new ArgumentNullException(nameof(device));
                
            if (GetDevice(device.Name) != null)
                throw new InvalidOperationException($"Device with name '{device.Name}' already exists");
                
            Devices.Add(device);
            Console.WriteLine($"Device added to repository: {device.Name}");
        }
        
        public void RemoveDevice(string deviceName)
        {
            var device = GetDevice(deviceName);
            if (device != null)
            {
                Devices.Remove(device);
                Console.WriteLine($"Device removed from repository: {deviceName}");
            }
        }
        
        public List<IDevice> GetAllDevices()
        {
            return new List<IDevice>(Devices);
        }
    }
}
EOF

# 6. DeviceManager.cs
cat > src/SystemCore/DeviceManager.cs << 'EOF'
using System;
using System.Threading;
using WaterTreatmentSCADA.Core.Interfaces;

namespace WaterTreatmentSCADA.SystemCore
{
    /// <summary>
    /// Central manager for all devices
    /// </summary>
    public class DeviceManager
    {
        private IDeviceRepository repository;
        private Timer updateTimer;
        private int updateIntervalMs = 1000;
        
        public DeviceManager(IDeviceRepository repository)
        {
            this.repository = repository ?? throw new ArgumentNullException(nameof(repository));
        }
        
        public void InitializeAllDevices()
        {
            foreach (var device in repository.GetAllDevices())
            {
                device.Initialize();
            }
            Console.WriteLine("All devices initialized");
        }
        
        public void StartAllDevices()
        {
            foreach (var device in repository.GetAllDevices())
            {
                device.Start();
            }
            
            updateTimer = new Timer(UpdateAllDevicesCallback, null, 0, updateIntervalMs);
            Console.WriteLine("All devices started");
        }
        
        public void StopAllDevices()
        {
            updateTimer?.Dispose();
            
            foreach (var device in repository.GetAllDevices())
            {
                device.Stop();
            }
            Console.WriteLine("All devices stopped");
        }
        
        public void UpdateAllDevices()
        {
            foreach (var device in repository.GetAllDevices())
            {
                try
                {
                    device.Update();
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error updating {device.Name}: {ex.Message}");
                }
            }
        }
        
        private void UpdateAllDevicesCallback(object state)
        {
            UpdateAllDevices();
        }
        
        public IDevice GetDevice(string deviceName)
        {
            return repository.GetDevice(deviceName);
        }
        
        public void PrintSystemTelemetry()
        {
            Console.WriteLine("\n=== SYSTEM TELEMETRY ===");
            foreach (var device in repository.GetAllDevices())
            {
                var telemetry = device.GetTelemetryData();
                Console.WriteLine($"\n{device.Name}:");
                foreach (var kvp in telemetry)
                {
                    Console.WriteLine($"  {kvp.Key}: {kvp.Value}");
                }
            }
            Console.WriteLine("========================\n");
        }
    }
}
EOF

echo "✅ All core files created!"
echo ""
echo "Created files:"
echo "  - src/Core/Base/BaseDevice.cs"
echo "  - src/Core/Base/ControllableDevice.cs"
echo "  - src/Core/Utilities/BinaryIO.cs"
echo "  - src/Core/Models/DeviceCommand.cs"
echo "  - src/SystemCore/DeviceRepository.cs"
echo "  - src/SystemCore/DeviceManager.cs"
echo ""
echo "Now delete the old interface files:"
echo "  rm src/Core/Interfaces/ISensor.cs"
echo "  rm src/Core/Interfaces/IControllableDevice.cs"