using System;
using System.Windows.Controls;
using System.Windows.Media;
using WaterTreatmentSCADA.Devices.Sensors;

namespace WaterTreatmentSCADA.GUI.Panels
{
    // Panel for monitoring pipeline/filter pressure
    public partial class PressureMonitoringPanel : UserControl
    {
        private PressureSensor? pressureSensor;

        public PressureMonitoringPanel()
        {
            InitializeComponent();
        }

        // Initialize with pressure sensor and subscribe to events
        public void Initialize(PressureSensor sensor)
        {
            pressureSensor = sensor;
            
            if (pressureSensor != null)
            {
                pressureSensor.OnReadingChange += OnPressureReadingChanged;
            }

            UpdateDisplay();
        }

        // Handle pressure reading changes - update on UI thread
        private void OnPressureReadingChanged(object? sender, double pressureValue)
        {
            Dispatcher.Invoke(() =>
            {
                UpdatePressureDisplay(pressureValue);
            });
        }

        // Update pressure display with color coding based on thresholds
        private void UpdatePressureDisplay(double pressureValue)
        {
            PressureValueText.Text = pressureValue.ToString("F2") + " bar";

            if (pressureValue < PressureSensor.MinSafePressure) // < 1.5 bar
            {
                // Red for critically low pressure (leak or pump failure)
                PressureValueBorder.Background = new SolidColorBrush(Color.FromRgb(44, 44, 44));
                PressureValueText.Foreground = new SolidColorBrush(Color.FromRgb(231, 76, 60)); // Red
                PressureIndicator.Fill = new SolidColorBrush(Color.FromRgb(231, 76, 60));
                PressureStatusText.Text = "🚨 CRITICAL LOW: Check for leaks";
            }
            else if (pressureValue >= PressureSensor.MinSafePressure && pressureValue < PressureSensor.NormalPressureLow) // 1.5-2.0 bar
            {
                // Yellow for low pressure
                PressureValueBorder.Background = new SolidColorBrush(Color.FromRgb(44, 44, 44));
                PressureValueText.Foreground = new SolidColorBrush(Color.FromRgb(241, 196, 15)); // Yellow
                PressureIndicator.Fill = new SolidColorBrush(Color.FromRgb(241, 196, 15));
                PressureStatusText.Text = "⚠ Low Pressure";
            }
            else if (pressureValue >= PressureSensor.NormalPressureLow && pressureValue <= PressureSensor.NormalPressureHigh) // 2.0-2.6 bar
            {
                // Green for normal pressure
                PressureValueBorder.Background = new SolidColorBrush(Color.FromRgb(44, 44, 44));
                PressureValueText.Foreground = new SolidColorBrush(Color.FromRgb(46, 204, 113)); // Green
                PressureIndicator.Fill = new SolidColorBrush(Color.FromRgb(46, 204, 113));
                PressureStatusText.Text = "Normal Pressure";
            }
            else if (pressureValue > PressureSensor.NormalPressureHigh && pressureValue < PressureSensor.CriticalPressure) // 2.7-2.9 bar
            {
                // Yellow for elevated pressure
                PressureValueBorder.Background = new SolidColorBrush(Color.FromRgb(44, 44, 44));
                PressureValueText.Foreground = new SolidColorBrush(Color.FromRgb(241, 196, 15)); // Yellow
                PressureIndicator.Fill = new SolidColorBrush(Color.FromRgb(241, 196, 15));
                PressureStatusText.Text = $"⚠ Elevated Pressure ({pressureValue:F2} bar)";
            }
            else // >= 3.0 bar
            {
                // Red for critical high pressure
                PressureValueBorder.Background = new SolidColorBrush(Color.FromRgb(44, 44, 44));
                PressureValueText.Foreground = new SolidColorBrush(Color.FromRgb(231, 76, 60)); // Red
                PressureIndicator.Fill = new SolidColorBrush(Color.FromRgb(231, 76, 60));
                PressureStatusText.Text = $"🚨 CRITICAL HIGH: Possible blockage ({pressureValue:F2} bar)";
            }
        }

        // Initial display update
        private void UpdateDisplay()
        {
            if (pressureSensor != null)
            {
                UpdatePressureDisplay(pressureSensor.CurrentReading);
            }
        }

        // Cleanup to prevent memory leaks
        public void Cleanup()
        {
            if (pressureSensor != null)
            {
                pressureSensor.OnReadingChange -= OnPressureReadingChanged;
            }
        }
    }
}