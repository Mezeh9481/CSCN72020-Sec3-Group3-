using System;
using System.IO;

namespace AquaVisionSCADA.IO
{
    /// <summary>
    /// BinaryIO implements the IFileSimulator interface to handle
    /// reading and writing of simulated device data.
    /// </summary>
    public class BinaryIO : IFileSimulator
    {
        public string FilePath { get; set; }

        public BinaryIO(string filePath)
        {
            FilePath = filePath;
        }

        /// <summary>
        /// Reads one line of data from the specified file.
        /// Returns null if the file does not exist or cannot be read.
        /// </summary>
        public string ReadLine()
        {
            try
            {
                using (StreamReader reader = new StreamReader(FilePath))
                {
                    string line = reader.ReadLine();
                    return line;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Error] Failed to read from file: {ex.Message}");
                return null;
            }
        }

        /// <summary>
        /// Writes a single line of data to the specified file.
        /// If the file does not exist, it will be created.
        /// </summary>
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
