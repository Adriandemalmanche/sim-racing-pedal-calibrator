using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using SimRacingPedalCalibrator.Services;
using SimRacingPedalCalibrator.Models;
using Windows.UI.Core;

namespace SimRacingPedalCalibrator
{
    public sealed partial class MainWindow : Window
    {
        private readonly DirectInputService _directInputService;
        private readonly RegistryService _registryService;
        private bool _isCalibrating = false;
        private CalibrationData _calibrationData = new();

        public MainWindow()
        {
            this.InitializeComponent();
            ExtendsContentIntoTitleBar = true;
            SetTitleBar(TitleBarGrid);

            _directInputService = new DirectInputService();
            _registryService = new RegistryService();

            Loaded += MainWindow_Loaded;
        }

        private async void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            await InitializeAsync();
        }

        private async Task InitializeAsync()
        {
            try
            {
                // Load calibration from registry
                _calibrationData = _registryService.LoadCalibration();
                
                // Update UI with loaded values
                BrakeControl.SetCalibration(_calibrationData.Brake);
                ThrottleControl.SetCalibration(_calibrationData.Throttle);
                ClutchControl.SetCalibration(_calibrationData.Clutch);

                // Initialize DirectInput device
                if (_directInputService.Initialize())
                {
                    DeviceStatusText.Text = "Device Connected";
                    // Start polling for values
                    StartPollng();
                }
                else
                {
                    DeviceStatusText.Text = "Device Not Found";
                }
            }
            catch (Exception ex)
            {
                DeviceStatusText.Text = $"Error: {ex.Message}";
            }
        }

        private void StartPollng()
        {
            var timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromMilliseconds(16); // ~60 FPS
            timer.Tick += (s, e) =>
            {
                var values = _directInputService.GetAxisValues();
                if (values != null)
                {
                    DispatcherQueue.TryEnqueue(() =>
                    {
                        BrakeControl.SetRawValue(values.Brake);
                        ThrottleControl.SetRawValue(values.Throttle);
                        ClutchControl.SetRawValue(values.Clutch);

                        if (_isCalibrating)
                        {
                            BrakeControl.UpdateCalibrationData(values.Brake);
                            ThrottleControl.UpdateCalibrationData(values.Throttle);
                            ClutchControl.UpdateCalibrationData(values.Clutch);
                        }
                    });
                }
            };
            timer.Start();
        }

        private void OnCalibrateClick(object sender, RoutedEventArgs e)
        {
            _isCalibrating = !_isCalibrating;
            
            if (_isCalibrating)
            {
                CalibrateButton.Content = "Stop Calibration";
                BrakeControl.ResetCalibration();
                ThrottleControl.ResetCalibration();
                ClutchControl.ResetCalibration();
            }
            else
            {
                CalibrateButton.Content = "Start Calibration";
            }
        }

        private void OnSaveClick(object sender, RoutedEventArgs e)
        {
            try
            {
                _calibrationData.Brake = BrakeControl.GetCalibration();
                _calibrationData.Throttle = ThrottleControl.GetCalibration();
                _calibrationData.Clutch = ClutchControl.GetCalibration();

                _registryService.SaveCalibration(_calibrationData);

                var dialog = new ContentDialog
                {
                    Title = "Success",
                    Content = "Calibration saved to registry successfully!",
                    CloseButtonText = "OK",
                    XamlRoot = Content.XamlRoot
                };
                _ = dialog.ShowAsync();
            }
            catch (Exception ex)
            {
                var dialog = new ContentDialog
                {
                    Title = "Error",
                    Content = $"Failed to save calibration: {ex.Message}",
                    CloseButtonText = "OK",
                    XamlRoot = Content.XamlRoot
                };
                _ = dialog.ShowAsync();
            }
        }

        private void OnResetClick(object sender, RoutedEventArgs e)
        {
            _calibrationData = new CalibrationData();
            BrakeControl.SetCalibration(_calibrationData.Brake);
            ThrottleControl.SetCalibration(_calibrationData.Throttle);
            ClutchControl.SetCalibration(_calibrationData.Clutch);
        }
    }
}
