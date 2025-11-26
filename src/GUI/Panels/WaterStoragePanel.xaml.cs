using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using WaterTreatmentSCADA.Devices.Sensors;

namespace WaterTreatmentSCADA.GUI.Panels
{
    // Panel for monitoring filtration sensor turbidity
    public partial class WaterStoragePanel : UserControl
    {
        private StorageSensor? storageSensor;

        private const double MinStorage = 0.0;
        private const double CriticalStorage = 950.0;
        private const double MaxStorage = 1000.0;
        public WaterStoragePanel()
        {
            InitializeComponent();
        }

        // Initialize with sensor and subscribe to events
       public void Initialize(StorageSensor sensor)
        {
            storageSensor = sensor;

            // Subscribe to events
            if (storageSensor != null)
            {
                storageSensor.OnReadingChange += OnStorageReadingChanged;
            }

            // Show initial values
            UpdateDisplay();
        }

        // Handle reading changes - update on UI thread
        private void OnStorageReadingChanged(object? sender, double storageValue)
        {
            Dispatcher.Invoke(() =>
            {
                UpdateStorageDisplay(storageValue);
            });
        }

        // Update Storage display with color coding
        private void UpdateStorageDisplay(double storageValue)
        {
            StorageValueText.Text = storageValue.ToString("F2") + " L";


            if (storageValue >= MinStorage && storageValue <= CriticalStorage && storageValue <= MaxStorage)
            {
                // Blue for safe
                StorageValueBorder.Background = new SolidColorBrush(Color.FromRgb(52, 152, 219));
                SafeRangeIndicator.Fill = new SolidColorBrush(Color.FromRgb(46, 204, 113)); 
                SafeRangeText.Text = "Tank level within safe range";
            }
            else if (storageValue >= MinStorage && storageValue >= CriticalStorage && storageValue <= MaxStorage)
            {
                // Yellow for warning
                StorageValueBorder.Background = new SolidColorBrush(Color.FromRgb(241, 196, 15));
                SafeRangeIndicator.Fill = new SolidColorBrush(Color.FromRgb(241, 196, 15));
                SafeRangeText.Text = $"⚠ HIGH WATER LEVEL ({storageValue:F2} L)";
            }
            else if (storageValue >= MinStorage && storageValue >= CriticalStorage && storageValue >= MaxStorage)
            {
                // Red for overflow
                StorageValueBorder.Background = new SolidColorBrush(Color.FromRgb(231, 76, 60));
                SafeRangeIndicator.Fill = new SolidColorBrush(Color.FromRgb(231, 76, 60));
                SafeRangeText.Text = $"⚠ WATER OVERFLOW ({storageValue:F2} L)";
            }
        }

        // Initial display update
        private void UpdateDisplay()
        {
            if (storageSensor != null)
            {
                UpdateStorageDisplay(storageSensor.CurrentReading);
            }

        }
    }
}

