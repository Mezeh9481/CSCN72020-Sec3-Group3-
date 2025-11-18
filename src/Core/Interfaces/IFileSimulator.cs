namespace WaterTreatmentSCADA.Core.Interfaces
{
    public interface IFileSimulator
    {
        string FilePath { get; }
        string? ReadLine();  // Returns null when EOF
        void WriteLine(string data);
        void Reset();
        bool IsEndOfFile { get; }
    }
}
