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
