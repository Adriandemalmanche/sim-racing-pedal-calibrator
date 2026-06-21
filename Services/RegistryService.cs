using Microsoft.Win32;
using SimRacingPedalCalibrator.Models;

namespace SimRacingPedalCalibrator.Services
{
    public class RegistryService
    {
        private const string RegistryPath = @"HKEY_CURRENT_USER\System\CurrentControlSet\Control\MediaProperties\PrivateProperties\DirectInput\VID_16C0&PID_05DF\Calibration\0\Type\Axes";

        public CalibrationData LoadCalibration()
        {
            var calibration = new CalibrationData();

            try
            {
                // Axis 0 - Brake
                var brakeData = Registry.GetValue($"{RegistryPath}\\0", "Calibration", null);
                if (brakeData is byte[] brakeBytess)
                {
                    var values = BytesToCalibration(brakeBytess);
                    calibration.Brake = values;
                }

                // Axis 1 - Throttle
                var throttleData = Registry.GetValue($"{RegistryPath}\\1", "Calibration", null);
                if (throttleData is byte[] throttleBytes)
                {
                    var values = BytesToCalibration(throttleBytes);
                    calibration.Throttle = values;
                }

                // Axis 2 - Clutch
                var clutchData = Registry.GetValue($"{RegistryPath}\\2", "Calibration", null);
                if (clutchData is byte[] clutchBytes)
                {
                    var values = BytesToCalibration(clutchBytes);
                    calibration.Clutch = values;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error loading calibration from registry: {ex.Message}");
            }

            return calibration;
        }

        public void SaveCalibration(CalibrationData calibration)
        {
            try
            {
                using (var key = Registry.CurrentUser.OpenSubKey($"System\\CurrentControlSet\\Control\\MediaProperties\\PrivateProperties\\DirectInput\\VID_16C0&PID_05DF\\Calibration\\0\\Type\\Axes", writable: true))
                {
                    if (key == null)
                        throw new InvalidOperationException("Registry key not found. Please ensure the device is connected.");

                    // Save Brake (Axis 0)
                    var brakeBytes = CalibrationToBytes(calibration.Brake);
                    using (var axis0 = key.OpenSubKey("0", writable: true) ?? key.CreateSubKey("0"))
                    {
                        axis0?.SetValue("Calibration", brakeBytes, RegistryValueKind.Binary);
                    }

                    // Save Throttle (Axis 1)
                    var throttleBytes = CalibrationToBytes(calibration.Throttle);
                    using (var axis1 = key.OpenSubKey("1", writable: true) ?? key.CreateSubKey("1"))
                    {
                        axis1?.SetValue("Calibration", throttleBytes, RegistryValueKind.Binary);
                    }

                    // Save Clutch (Axis 2)
                    var clutchBytes = CalibrationToBytes(calibration.Clutch);
                    using (var axis2 = key.OpenSubKey("2", writable: true) ?? key.CreateSubKey("2"))
                    {
                        axis2?.SetValue("Calibration", clutchBytes, RegistryValueKind.Binary);
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error saving calibration to registry: {ex.Message}");
                throw;
            }
        }

        private AxisCalibration BytesToCalibration(byte[] data)
        {
            if (data.Length < 12)
                throw new ArgumentException("Invalid calibration data");

            // Convert 4-byte little-endian integers
            int min = BitConverter.ToInt32(data, 0);
            int center = BitConverter.ToInt32(data, 4);
            int max = BitConverter.ToInt32(data, 8);

            return new AxisCalibration(min, center, max);
        }

        private byte[] CalibrationToBytes(AxisCalibration calibration)
        {
            var bytes = new byte[12];
            
            // Convert to 4-byte little-endian integers
            BitConverter.GetBytes(calibration.Min).CopyTo(bytes, 0);
            BitConverter.GetBytes(calibration.Center).CopyTo(bytes, 4);
            BitConverter.GetBytes(calibration.Max).CopyTo(bytes, 8);

            return bytes;
        }
    }
}
