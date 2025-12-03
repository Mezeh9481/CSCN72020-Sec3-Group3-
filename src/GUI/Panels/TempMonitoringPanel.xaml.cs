using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using WaterTreatmentSCADA.Devices.Sensors;
using WaterTreatmentSCADA.Devices.Devices;

namespace WaterTreatmentSCADA.GUI.Panels
{
    // Panel for monitoring Temp levels and showing chemical doser status
    public partial class TempMonitoringPanel : UserControl
    {
        private TempSensor? TempSensor;

        // Safe Temp range (6.5 - 8.5)
         private const double MinTemp = 22.0;

        private const double WarningTemp = 23.0;



        public TempMonitoringPanel()
        {
            InitializeComponent();
        }

        // Initialize with sensor and doser, subscribe to their events
        public void Initialize(TempSensor sensor)
        {
            TempSensor = sensor;
            
            // Subscribe to events
            if (TempSensor != null)
            {
                TempSensor.OnReadingChange += OnTempReadingChanged;
            }


            // Show initial values
            UpdateDisplay();
        }

        // Handle Temp reading changes - update on UI thread
        private void OnTempReadingChanged(object? sender, double TempValue)
        {
            Dispatcher.Invoke(() =>
            {
                UpdateTempDisplay(TempValue);
            });
        }

        // Update Temp display with color coding
         private void UpdateTempDisplay(double TempValue)
        {
            TempValueText.Text = TempValue.ToString("F2") + " °C";


            if (TempValue < MinTemp)
            {
                // Blue for safe
                TempValueBorder.Background = new SolidColorBrush(Color.FromRgb(52, 152, 219));
                SafeRangeIndicator.Fill = new SolidColorBrush(Color.FromRgb(46, 204, 113)); 
                SafeRangeText.Text = "Temp Normal";
            }
            else if (TempValue >= MinTemp && TempValue <= WarningTemp)
            {
                // Yellow for warning
                TempValueBorder.Background = new SolidColorBrush(Color.FromRgb(241, 196, 15));
                SafeRangeIndicator.Fill = new SolidColorBrush(Color.FromRgb(241, 196, 15));
                SafeRangeText.Text = $"⚠ Higher than normal Temp ({TempValue:F2} °C)";
            }
            else if (TempValue > WarningTemp)
            {
                // Red for overflow
                TempValueBorder.Background = new SolidColorBrush(Color.FromRgb(231, 76, 60));
                SafeRangeIndicator.Fill = new SolidColorBrush(Color.FromRgb(231, 76, 60));
                SafeRangeText.Text = $"⚠ TEMPERATURE EXCEEDS CRITICAL LEVELS ({TempValue:F2} °C)";
            }
        }

        // Initial display update
        private void UpdateDisplay()
        {
            if (TempSensor != null)
            {
                UpdateTempDisplay(TempSensor.CurrentReading);
            }
        }
    }
}

