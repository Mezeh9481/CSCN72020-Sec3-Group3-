using System;
using System.IO;
using System.Windows;
using WaterTreatmentSCADA.GUI.Panels;
using WaterTreatmentSCADA.SystemCore;



namespace WaterTreatmentSCADA.GUI
{
    // Main window with all monitoring panels
    public partial class MainWindow : Window
    {
        private SystemController? systemController;

        public MainWindow()
        {
            InitializeComponent();
            InitializeSystem();
        }

        // Initialize system with all devices and sensors
        private void InitializeSystem()
        {
            try
            {
                // Get path to simulation data files
                string baseDir = AppDomain.CurrentDomain.BaseDirectory;
                string projectRoot = Path.GetFullPath(Path.Combine(baseDir, "..", "..", "..", "..", ".."));
                string dataPath = Path.Combine(projectRoot, "data", "simulations");

                // Create and initialize system controller
                systemController = new SystemController();
                systemController.Initialize(dataPath);


                // Initialize with devices
                pressureMonitoringPanel.Initialize(systemController.PressureSensor!);
                tempMonitoringPanel.Initialize(systemController.TempSensor!); 
                waterStoragePanel.Initialize(systemController.StorageSensor!);
                phMonitoringPanel.Initialize(systemController.PHSensor!, systemController.ChemicalDoser!);
                intakePumpPanel.Initialize(systemController.IntakePump!);
                chlorinePumpPanel.Initialize(systemController.ChlorinePump!);
                filtrationSensorPanel.Initialize(systemController.FiltrationSensor!);
                
  
                // Subscribe to system events (for logging/debugging)
                if (systemController != null)
                {
                    systemController.OnSystemEvent += OnSystemEventReceived;
                    // Start the system
                    systemController.Start();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error initializing system: {ex.Message}", 
                    "Initialization Error", 
                    MessageBoxButton.OK, 
                    MessageBoxImage.Error);
            }
        }

        // Handle system events (currently just for logging - can add log viewer UI later)
        private void OnSystemEventReceived(object? sender, SystemEvent e)
        {
            // Events are already logged to console by SystemController
            // Could add log viewer UI here if needed
        }

        // Cleanup when window closes
        protected override void OnClosed(EventArgs e)
        {
            systemController?.Shutdown();
            base.OnClosed(e);
        }

        // Access to system controller (in case needed)
        public SystemController? SystemController => systemController;
    }
}

