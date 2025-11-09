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
