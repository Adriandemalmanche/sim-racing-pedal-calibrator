using SharpDX.DirectInput;
using SimRacingPedalCalibrator.Models;

namespace SimRacingPedalCalibrator.Services
{
    public class DirectInputService
    {
        private DirectInput? _directInput;
        private Joystick? _joystick;
        private const int VendorId = 0x16C0;
        private const int ProductId = 0x05DF;

        public bool Initialize()
        {
            try
            {
                _directInput = new DirectInput();
                
                // Find device with matching VID/PID
                var devices = _directInput.GetDevices(DeviceType.Joystick, DeviceEnumerationFlags.AllDevices);
                
                foreach (var device in devices)
                {
                    if (device.ProductId == ProductId && device.VendorId == VendorId)
                    {
                        _joystick = new Joystick(_directInput, device.InstanceGuid);
                        _joystick.Properties.BufferSize = 128;
                        _joystick.Acquire();
                        return true;
                    }
                }

                return false;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"DirectInput initialization error: {ex.Message}");
                return false;
            }
        }

        public AxisValues? GetAxisValues()
        {
            try
            {
                if (_joystick == null)
                    return null;

                _joystick.Poll();
                var state = _joystick.GetCurrentState();

                return new AxisValues
                {
                    Brake = state.X,      // X axis
                    Throttle = state.Y,   // Y axis
                    Clutch = state.Z      // Z axis
                };
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error reading joystick state: {ex.Message}");
                return null;
            }
        }

        public void Dispose()
        {
            if (_joystick != null)
            {
                _joystick.Unacquire();
                _joystick.Dispose();
            }

            _directInput?.Dispose();
        }
    }
}
