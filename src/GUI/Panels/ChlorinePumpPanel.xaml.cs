using System;
using System.Windows;
using System.Windows.Controls;
using WaterTreatmentSCADA.Devices.Devices;

namespace WaterTreatmentSCADA.GUI.Panels
{
    // Panel for controlling chlorine pump and monitoring dosing rate
    public partial class ChlorinePumpPanel : UserControl
    {
        private ChlorinePump? chlorinePump;
        private bool isUpdatingFromDevice = false; // Prevents slider feedback loop

        public ChlorinePumpPanel()
        {
            InitializeComponent();
        }

        // Initialize with pump device and subscribe to events
        public void Initialize(ChlorinePump pump)
        {
            chlorinePump = pump;

            // Subscribe to pump events
            if (chlorinePump != null)
            {
                chlorinePump.OnStateChange += OnPumpStateChanged;
                chlorinePump.OnDosingRateChange += OnDosingRateChanged;
                chlorinePump.OnChlorineLevelChange += OnChlorineLevelChanged;
            }

            UpdateDisplay();
        }

        // Handle pump state changes
        private void OnPumpStateChanged(object? sender, bool isOn)
        {
            Dispatcher.Invoke(() =>
            {
                UpdatePowerButton(isOn);
                UpdateStatus(isOn);
            });
        }

        // Handle dosing rate changes
        private void OnDosingRateChanged(object? sender, double dosingRate)
        {
            Dispatcher.Invoke(() =>
            {
                UpdateDosingRateDisplay(dosingRate);
            });
        }

        // Handle chlorine level changes
        private void OnChlorineLevelChanged(object? sender, double chlorineLevel)
        {
            Dispatcher.Invoke(() =>
            {
                UpdateChlorineLevelDisplay(chlorineLevel);
            });
        }

        // Handle power button click
        private void PowerButton_Click(object sender, RoutedEventArgs e)
        {
            if (chlorinePump == null) return;

            if (chlorinePump.IsOn)
            {
                chlorinePump.TurnOff();
            }
            else
            {
                chlorinePump.TurnOn();
            }
        }

        // Handle slider value change
        private void DosingRateSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (chlorinePump == null || isUpdatingFromDevice) return;

            try
            {
                chlorinePump.SetDosingRate(e.NewValue);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error setting dosing rate: {ex.Message}", 
                    "Error", 
                    MessageBoxButton.OK, 
                    MessageBoxImage.Error);
            }
        }

        // Update button text and color based on pump state
        private void UpdatePowerButton(bool isOn)
        {
            if (isOn)
            {
                PowerButton.Content = "TURN OFF";
                PowerButton.Background = new System.Windows.Media.SolidColorBrush(
                    System.Windows.Media.Color.FromRgb(231, 76, 60)); // Red
                DosingRateSlider.IsEnabled = true;
            }
            else
            {
                PowerButton.Content = "TURN ON";
                PowerButton.Background = new System.Windows.Media.SolidColorBrush(
                    System.Windows.Media.Color.FromRgb(39, 174, 96)); // Green
                DosingRateSlider.IsEnabled = false;
            }
        }

        // Update chlorine level display
        private void UpdateChlorineLevelDisplay(double chlorineLevel)
        {
            ChlorineLevelText.Text = $"{chlorineLevel:F2} ppm";
        }

        // Update dosing rate display and slider
        private void UpdateDosingRateDisplay(double dosingRate)
        {
            DosingRateText.Text = $"{dosingRate:F1}%";
            
            // Update slider without triggering event (prevent feedback loop)
            isUpdatingFromDevice = true;
            DosingRateSlider.Value = dosingRate;
            isUpdatingFromDevice = false;
        }

        // Update status text
        private void UpdateStatus(bool isOn)
        {
            if (isOn)
            {
                StatusText.Text = $"Pump is ON - Dosing chlorine at {chlorinePump?.DosingRate:F1}% | Level: {chlorinePump?.ChlorineLevel:F2} ppm";
            }
            else
            {
                StatusText.Text = $"Pump is OFF - No chlorine dosing | Level: {chlorinePump?.ChlorineLevel:F2} ppm";
            }
        }

        // Initial display update
        private void UpdateDisplay()
        {
            if (chlorinePump != null)
            {
                UpdatePowerButton(chlorinePump.IsOn);
                UpdateChlorineLevelDisplay(chlorinePump.ChlorineLevel);
                UpdateDosingRateDisplay(chlorinePump.DosingRate);
                UpdateStatus(chlorinePump.IsOn);
            }
        }
    }
}