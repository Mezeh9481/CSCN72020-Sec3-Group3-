using System;
using System.IO;

namespace WaterTreatmentSCADA.SystemCore
{
    // Simple file I/O for reading simulation data (not currently used - BinaryIO in Core is used instead)
    public class BinarioIO
    {
        public string FilePath { get; set; }

        public BinarioIO(string filePath)
        {
            FilePath = filePath;
        }

        // Read one line from file (returns null on error)
        public string? ReadLine()
        {
            try
            {
                using (StreamReader reader = new StreamReader(FilePath))
                {
                    string? line = reader.ReadLine();
                    return line;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Error] Failed to read from file: {ex.Message}");
                return null;
            }
        }

        // Write one line to file
        public void WriteLine(string data)
        {
            try
            {
                using (StreamWriter writer = new StreamWriter(FilePath, append: true))
                {
                    writer.WriteLine(data);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Error] Failed to write to file: {ex.Message}");
            }
        }
    }
}
