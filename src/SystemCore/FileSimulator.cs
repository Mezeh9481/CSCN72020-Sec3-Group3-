using System;

namespace AquaVisionSCADA.IO
{
    /// <summary>
    /// Interface that defines file simulation behavior for reading and writing data.
    /// </summary>
    public interface IFileSimulator
    {
        /// <summary>
        /// Gets or sets the file path for the simulated data source.
        /// </summary>
        string FilePath { get; set; }

        /// <summary>
        /// Reads a line of simulated data from the file.
        /// </summary>
        /// <returns>String containing one line of data.</returns>
        string ReadLine();

        /// <summary>
        /// Writes a line of data to the file (used for logging or simulation output).
        /// </summary>
        /// <param name="data">The string data to write.</param>
        void WriteLine(string data);
    }
}
