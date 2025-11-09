using System;
using System.Collections.Generic;
using WaterTreatmentSCADA.Core.Base;
using WaterTreatmentSCADA.Core.Interfaces;  

namespace TestConsole
{
    public class TestDevice : ControllableDevice
    {
        public double TestValue { get; private set; }
        
        public TestDevice(string name, string simulationFile) 
            : base(name, "TestDevice", simulationFile)
        {
            TestValue = 0.0;
        }
        
        public override void Update()
        {
            // Get data from simulation file
            string dataLine = GetFileInfo();
            
            if (!string.IsNullOrEmpty(dataLine))
            {
                // Simple parsing - assumes CSV format: timestamp,value
                var parts = dataLine.Split(',');
                if (parts.Length >= 2)
                {
                    TestValue = double.Parse(parts[1]);
                    LastUpdate = DateTime.Now;
                }
            }
        }
        
        public override void SetConfig(string configName, object value)
        {
            Console.WriteLine($"SetConfig called: {configName} = {value}");
        }
        
        public override object GetConfig(string configName)
        {
            if (configName == "testValue")
                return TestValue;
            return null;
        }
        
        public override Dictionary<string, object> GetTelemetryData()
        {
            return new Dictionary<string, object>
            {
                { "name", Name },
                { "type", DeviceType },
                { "status", Status.ToString() },
                { "testValue", TestValue },
                { "isRunning", IsRunning },
                { "lastUpdate", LastUpdate.ToString("yyyy-MM-dd HH:mm:ss") }
            };
        }
    }
}