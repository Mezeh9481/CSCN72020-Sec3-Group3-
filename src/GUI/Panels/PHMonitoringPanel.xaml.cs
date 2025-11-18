using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using WaterTreatmentSCADA.Devices.Sensors;
using WaterTreatmentSCADA.Devices.Devices;

namespace WaterTreatmentSCADA.GUI.Panels
{
    // Panel for monitoring pH levels and showing chemical doser status
    public partial class PHMonitoringPanel : UserControl
    {
        private pHSensor? phSensor;
        private ChemicalDoser? chemicalDoser;

        // Safe pH range (6.5 - 8.5)
        private const double LowerSafePH = 6.5;
        private const double UpperSafePH = 8.5;

        public PHMonitoringPanel()
        {
            InitializeComponent();
        }

        // Initialize with sensor and doser, subscribe to their events
        public void Initialize(pHSensor sensor, ChemicalDoser doser)
        {
            phSensor = sensor;
            chemicalDoser = doser;

            // Subscribe to events
            if (phSensor != null)
            {
                phSensor.OnReadingChange += OnPHReadingChanged;
            }

            if (chemicalDoser != null)
            {
                chemicalDoser.OnStateChange += OnDoserStateChanged;
            }

            // Show initial values
            UpdateDisplay();
        }

        // Handle pH reading changes - update on UI thread
        private void OnPHReadingChanged(object? sender, double phValue)
        {
            Dispatcher.Invoke(() =>
            {
                UpdatePHDisplay(phValue);
            });
        }

        // Handle doser state changes - update on UI thread
        private void OnDoserStateChanged(object? sender, bool isActive)
        {
            Dispatcher.Invoke(() =>
            {
                UpdateDoserStatus(isActive);
            });
        }

        // Update pH display with color coding
        private void UpdatePHDisplay(double phValue)
        {
            PhValueText.Text = phValue.ToString("F2");

            // Color code based on safe range
            bool isSafe = phValue >= LowerSafePH && phValue <= UpperSafePH;
            
            if (isSafe)
            {
                // Blue for safe
                PhValueBorder.Background = new SolidColorBrush(Color.FromRgb(52, 152, 219));
                SafeRangeIndicator.Fill = new SolidColorBrush(Color.FromRgb(46, 204, 113)); // Green dot
                SafeRangeText.Text = "pH in safe range (6.5 - 8.5)";
            }
            else
            {
                // Red for unsafe
                PhValueBorder.Background = new SolidColorBrush(Color.FromRgb(231, 76, 60));
                SafeRangeIndicator.Fill = new SolidColorBrush(Color.FromRgb(231, 76, 60)); // Red dot
                SafeRangeText.Text = $"pH OUT OF RANGE! ({phValue:F2})";
            }
        }

        // Update doser status display
        private void UpdateDoserStatus(bool isActive)
        {
            if (isActive)
            {
                DoserStatusText.Text = "ACTIVE";
                DoserStatusBorder.Background = new SolidColorBrush(Color.FromRgb(231, 76, 60)); // Red
            }
            else
            {
                DoserStatusText.Text = "INACTIVE";
                DoserStatusBorder.Background = new SolidColorBrush(Color.FromRgb(149, 165, 166)); // Gray
            }
        }

        // Initial display update
        private void UpdateDisplay()
        {
            if (phSensor != null)
            {
                UpdatePHDisplay(phSensor.CurrentReading);
            }

            if (chemicalDoser != null)
            {
                UpdateDoserStatus(chemicalDoser.IsActive);
            }
        }
    }
}

