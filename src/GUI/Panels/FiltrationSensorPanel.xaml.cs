using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using WaterTreatmentSCADA.Devices.Sensors;

namespace WaterTreatmentSCADA.GUI.Panels
{
    // Panel for monitoring filtration sensor turbidity
    public partial class FiltrationSensorPanel : UserControl
    {
        private FiltrationSensor? filtrationSensor;

        // Color thresholds
        private const double GreenThreshold = 3.0;  // < 3 NTU = green
        private const double YellowThreshold = 5.0; // 3-5 NTU = yellow, > 5 = red

        public FiltrationSensorPanel()
        {
            InitializeComponent();
        }

        // Initialize with sensor and subscribe to events
        public void Initialize(FiltrationSensor sensor)
        {
            filtrationSensor = sensor;

            // Subscribe to sensor events
            if (filtrationSensor != null)
            {
                filtrationSensor.OnTurbidityChange += OnTurbidityChanged;
                filtrationSensor.OnThresholdAlert += OnThresholdAlert;
                filtrationSensor.OnThresholdCleared += OnThresholdCleared;
            }

            UpdateDisplay();
        }

        // Handle turbidity changes
        private void OnTurbidityChanged(object? sender, double turbidity)
        {
            Dispatcher.Invoke(() =>
            {
                UpdateTurbidityDisplay(turbidity);
            });
        }

        // Handle alert threshold exceeded
        private void OnThresholdAlert(object? sender, double turbidity)
        {
            Dispatcher.Invoke(() =>
            {
                ShowAlert(turbidity);
            });
        }

        // Handle alert cleared
        private void OnThresholdCleared(object? sender, double turbidity)
        {
            Dispatcher.Invoke(() =>
            {
                HideAlert();
            });
        }

        // Update turbidity display with color coding
        private void UpdateTurbidityDisplay(double turbidity)
        {
            TurbidityValueText.Text = turbidity.ToString("F2");
            StatusText.Text = $"Turbidity: {turbidity:F2} NTU";

            // Color code: green < 3, yellow 3-5, red > 5
            if (turbidity < GreenThreshold)
            {
                TurbidityGaugeBorder.Background = new SolidColorBrush(Color.FromRgb(46, 204, 113)); // Green
            }
            else if (turbidity < YellowThreshold)
            {
                TurbidityGaugeBorder.Background = new SolidColorBrush(Color.FromRgb(243, 156, 18)); // Yellow
            }
            else
            {
                TurbidityGaugeBorder.Background = new SolidColorBrush(Color.FromRgb(231, 76, 60)); // Red
            }
        }

        // Show alert banner
        private void ShowAlert(double turbidity)
        {
            AlertBanner.Visibility = Visibility.Visible;
            AlertText.Text = $"⚠️ HIGH TURBIDITY ALERT: {turbidity:F2} NTU exceeds threshold!";
        }

        // Hide alert banner
        private void HideAlert()
        {
            AlertBanner.Visibility = Visibility.Collapsed;
        }

        // Initial display update
        private void UpdateDisplay()
        {
            if (filtrationSensor != null)
            {
                UpdateTurbidityDisplay(filtrationSensor.CurrentTurbidity);
                ThresholdText.Text = $"Alert Threshold: {filtrationSensor.AlertThreshold:F1} NTU";

                if (filtrationSensor.IsAlertActive)
                {
                    ShowAlert(filtrationSensor.CurrentTurbidity);
                }
                else
                {
                    HideAlert();
                }
            }
        }
    }
}

