using System;
using System.IO;
using WaterTreatmentSCADA.Core.Interfaces;

namespace WaterTreatmentSCADA.Core.Utilities
{
    // File I/O for reading simulation data from CSV files
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
                
                // Skip header line if CSV file
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
        
        // Read one line from file
        public string? ReadLine()
        {
            if (reader != null && !reader.EndOfStream)
            {
                string? line = reader.ReadLine();
                IsEndOfFile = reader.EndOfStream;
                return line;
            }
            
            IsEndOfFile = true;
            return null;
        }
        
        // Write line to file (for logging)
        public void WriteLine(string data)
        {
            using (var logWriter = new StreamWriter(FilePath, true))
            {
                logWriter.WriteLine(data);
            }
        }
        
        // Reset file to beginning (for looping simulation)
        public void Reset()
        {
            if (reader != null)
            {
                reader.BaseStream.Seek(0, SeekOrigin.Begin);
                reader.DiscardBufferedData();
                
                // Skip header again if CSV
                if (FilePath.EndsWith(".csv"))
                {
                    reader.ReadLine();
                }
                
                IsEndOfFile = false;
            }
        }
        
        // Cleanup when object destroyed
        ~BinaryIO()
        {
            reader?.Dispose();
            fileStream?.Dispose();
        }
    }
}
