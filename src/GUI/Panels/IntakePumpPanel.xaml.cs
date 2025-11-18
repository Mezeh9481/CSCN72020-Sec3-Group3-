using System;
using System.Windows;
using System.Windows.Controls;
using WaterTreatmentSCADA.Devices.Devices;

namespace WaterTreatmentSCADA.GUI.Panels
{
    // Panel for controlling intake pump and monitoring flow rate
    public partial class IntakePumpPanel : UserControl
    {
        private IntakePump? intakePump;
        private bool isUpdatingFromDevice = false; // Prevents slider feedback loop

        public IntakePumpPanel()
        {
            InitializeComponent();
        }

        // Initialize with pump device and subscribe to events
        public void Initialize(IntakePump pump)
        {
            intakePump = pump;

            // Subscribe to pump events
            if (intakePump != null)
            {
                intakePump.OnStateChange += OnPumpStateChanged;
                intakePump.OnFlowRateChange += OnFlowRateChanged;
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

        // Handle flow rate changes
        private void OnFlowRateChanged(object? sender, double flowRate)
        {
            Dispatcher.Invoke(() =>
            {
                UpdateFlowRateDisplay(flowRate);
            });
        }

        // Handle power button click
        private void PowerButton_Click(object sender, RoutedEventArgs e)
        {
            if (intakePump == null) return;

            if (intakePump.IsOn)
            {
                intakePump.TurnOff();
            }
            else
            {
                intakePump.TurnOn();
            }
        }

        // Handle slider value change
        private void FlowRateSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (intakePump == null || isUpdatingFromDevice) return;

            try
            {
                intakePump.SetFlowRate(e.NewValue);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error setting flow rate: {ex.Message}", 
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
                FlowRateSlider.IsEnabled = true;
            }
            else
            {
                PowerButton.Content = "TURN ON";
                PowerButton.Background = new System.Windows.Media.SolidColorBrush(
                    System.Windows.Media.Color.FromRgb(52, 152, 219)); // Blue
                FlowRateSlider.IsEnabled = false;
            }
        }

        // Update flow rate display and slider
        private void UpdateFlowRateDisplay(double flowRate)
        {
            FlowRateText.Text = $"{flowRate:F1}%";
            
            // Update slider without triggering event (prevent feedback loop)
            isUpdatingFromDevice = true;
            FlowRateSlider.Value = flowRate;
            isUpdatingFromDevice = false;
        }

        // Update status text
        private void UpdateStatus(bool isOn)
        {
            if (isOn)
            {
                StatusText.Text = $"Pump is ON - Flow Rate: {intakePump?.FlowRate:F1}%";
            }
            else
            {
                StatusText.Text = "Pump is OFF";
            }
        }

        // Initial display update
        private void UpdateDisplay()
        {
            if (intakePump != null)
            {
                UpdatePowerButton(intakePump.IsOn);
                UpdateFlowRateDisplay(intakePump.FlowRate);
                UpdateStatus(intakePump.IsOn);
            }
        }
    }
}

