namespace WaterTreatmentSCADA.Core.Interfaces
{
    // Device status states
    public enum DeviceStatus
    {
        Offline,      // Device not communicating or powered off
        Online,       // Device operating normally
        Warning,      // Device operating with warnings
        Critical,     // Device in critical state - needs attention
        Error,        // Device encountered an error
        Maintenance   // Device in maintenance mode
    }
}
