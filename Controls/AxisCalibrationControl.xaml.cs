using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using SimRacingPedalCalibrator.Models;

namespace SimRacingPedalCalibrator
{
    public sealed partial class AxisCalibrationControl : UserControl
    {
        private AxisCalibration _calibration = new();
        private int _minSampled = int.MaxValue;
        private int _maxSampled = int.MinValue;

        public AxisCalibrationControl()
        {
            this.InitializeComponent();
        }

        public string AxisName
        {
            set => AxisNameText.Text = value;
        }

        public string AxisColor
        {
            set => ValueBar.Foreground = new Microsoft.UI.Xaml.Media.SolidColorBrush(Windows.UI.Color.FromArgb(255, 
                byte.Parse(value.Substring(1, 2), System.Globalization.NumberStyles.HexNumber),
                byte.Parse(value.Substring(3, 2), System.Globalization.NumberStyles.HexNumber),
                byte.Parse(value.Substring(5, 2), System.Globalization.NumberStyles.HexNumber)));
        }

        public void SetCalibration(AxisCalibration calibration)
        {
            _calibration = calibration;
            MinInput.Value = calibration.Min;
            MaxInput.Value = calibration.Max;
            DeadZoneSlider.Value = calibration.DeadZone;
            UpdateDisplay();
        }

        public AxisCalibration GetCalibration()
        {
            return new AxisCalibration(
                (int)MinInput.Value,
                (_calibration.Min + (int)MaxInput.Value) / 2,
                (int)MaxInput.Value,
                (int)DeadZoneSlider.Value);
        }

        public void SetRawValue(int rawValue)
        {
            CurrentValueText.Text = rawValue.ToString();
            ValueBar.Value = rawValue;
        }

        public void ResetCalibration()
        {
            _minSampled = int.MaxValue;
            _maxSampled = int.MinValue;
        }

        public void UpdateCalibrationData(int rawValue)
        {
            if (rawValue < _minSampled)
                _minSampled = rawValue;
            if (rawValue > _maxSampled)
                _maxSampled = rawValue;

            MinInput.Value = _minSampled;
            MaxInput.Value = _maxSampled;
        }

        private void UpdateDisplay()
        {
            MinValueText.Text = _calibration.Min.ToString();
            MaxValueText.Text = _calibration.Max.ToString();
            DeadZoneValue.Text = _calibration.DeadZone.ToString();
        }

        private void OnCalibrationValueChanged(object sender, RoutedEventArgs e)
        {
            UpdateDisplay();
        }

        private void OnDeadZoneChanged(object sender, RangeBaseValueChangedEventArgs e)
        {
            DeadZoneValue.Text = ((int)e.NewValue).ToString();
        }
    }
}
