namespace WaterTreatmentSCADA.Core.Interfaces
{
    /// <summary>
    /// Enumeration of possible device status states
    /// </summary>
    public enum DeviceStatus
    {
        /// <summary>
        /// Device is not communicating or powered off
        /// </summary>
        Offline,
        
        /// <summary>
        /// Device is operating normally
        /// </summary>
        Online,
        
        /// <summary>
        /// Device is operating but with warnings
        /// </summary>
        Warning,
        
        /// <summary>
        /// Device is in critical state requiring immediate attention
        /// </summary>
        Critical,
        
        /// <summary>
        /// Device has encountered an error
        /// </summary>
        Error,
        
        /// <summary>
        /// Device is in maintenance mode
        /// </summary>
        Maintenance
    }
}
