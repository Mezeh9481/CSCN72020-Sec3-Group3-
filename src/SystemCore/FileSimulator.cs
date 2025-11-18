using System;

namespace WaterTreatmentSCADA.SystemCore
{
    // Simple file simulator interface (not currently used - IFileSimulator in Core is used instead)
    public interface IFileSimulatorSimple
    {
        string FilePath { get; set; }
        string? ReadLine();
        void WriteLine(string data);
    }
}
