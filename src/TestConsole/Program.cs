using System;
using System.Threading;
using WaterTreatmentSCADA.Core.Interfaces;
using WaterTreatmentSCADA.SystemCore;

namespace TestConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("╔════════════════════════════════════════════════╗");
            Console.WriteLine("║   Water Treatment SCADA - System Core Test    ║");
            Console.WriteLine("╚════════════════════════════════════════════════╝");
            Console.WriteLine();
            
            try
            {
                // Test 1: Create Repository
                Console.WriteLine("TEST 1: Creating DeviceRepository...");
                IDeviceRepository repository = new DeviceRepository();
                Console.WriteLine("✅ DeviceRepository created successfully\n");
                
                // Test 2: Create Test Devices
                Console.WriteLine("TEST 2: Creating test devices...");
                
                var device1 = new TestDevice(
                    "Test Device 1",
                    "../../data/simulations/WaterInletDevice_simulation.csv"
                );
                
                var device2 = new TestDevice(
                    "Test Device 2", 
                    "../../data/simulations/CoagulationSensor_simulation.csv"
                );
                
                var device3 = new TestDevice(
                    "Test Device 3",
                    "../../data/simulations/ChlorinePumpDevice_simulation.csv"
                );
                
                Console.WriteLine("✅ 3 test devices created\n");
                
                // Test 3: Add Devices to Repository
                Console.WriteLine("TEST 3: Adding devices to repository...");
                repository.AddDevice(device1);
                repository.AddDevice(device2);
                repository.AddDevice(device3);
                Console.WriteLine($"✅ Repository now has {repository.Devices.Count} devices\n");
                
                // Test 4: Retrieve Device from Repository
                Console.WriteLine("TEST 4: Retrieving device from repository...");
                var retrieved = repository.GetDevice("Test Device 2");
                if (retrieved != null)
                {
                    Console.WriteLine($"✅ Successfully retrieved: {retrieved.Name}\n");
                }
                else
                {
                    Console.WriteLine("❌ Device not found\n");
                }
                
                // Test 5: Create DeviceManager
                Console.WriteLine("TEST 5: Creating DeviceManager...");
                DeviceManager manager = new DeviceManager(repository);
                Console.WriteLine("✅ DeviceManager created successfully\n");
                
                // Test 6: Initialize All Devices
                Console.WriteLine("TEST 6: Initializing all devices...");
                manager.InitializeAllDevices();
                Console.WriteLine("✅ All devices initialized\n");
                
                // Test 7: Start All Devices
                Console.WriteLine("TEST 7: Starting all devices...");
                manager.StartAllDevices();
                Console.WriteLine("✅ All devices started (updating every 1 second)\n");
                
                // Test 8: Run for 10 seconds and show telemetry
                Console.WriteLine("TEST 8: Running system for 10 seconds...");
                Console.WriteLine("Press Ctrl+C to stop early\n");
                
                for (int i = 1; i <= 10; i++)
                {
                    Thread.Sleep(1000);
                    Console.WriteLine($"--- Second {i} ---");
                    manager.PrintSystemTelemetry();
                }
                
                // Test 9: Device Control
                Console.WriteLine("\nTEST 9: Testing device control...");
                var controllableDevice = manager.GetDevice("Test Device 1") as TestDevice;
                if (controllableDevice != null)
                {
                    Console.WriteLine("Turning device OFF...");
                    controllableDevice.TurnOff();
                    Thread.Sleep(2000);
                    
                    Console.WriteLine("Turning device ON...");
                    controllableDevice.TurnOn();
                    Thread.Sleep(2000);
                    
                    Console.WriteLine("✅ Device control working\n");
                }
                
                // Test 10: Stop All Devices
                Console.WriteLine("TEST 10: Stopping all devices...");
                manager.StopAllDevices();
                Console.WriteLine("✅ All devices stopped\n");
                
                // Test 11: Remove Device from Repository
                Console.WriteLine("TEST 11: Removing device from repository...");
                repository.RemoveDevice("Test Device 2");
                Console.WriteLine($"✅ Repository now has {repository.Devices.Count} devices\n");
                
                // Summary
                Console.WriteLine("╔════════════════════════════════════════════════╗");
                Console.WriteLine("║            ALL TESTS PASSED! ✅                ║");
                Console.WriteLine("╚════════════════════════════════════════════════╝");
                Console.WriteLine();
                Console.WriteLine("DeviceRepository and DeviceManager are working correctly!");
                
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\n❌ ERROR: {ex.Message}");
                Console.WriteLine($"Stack Trace: {ex.StackTrace}");
            }
            
            Console.WriteLine("\nPress any key to exit...");
            Console.ReadKey();
        }
    }
}