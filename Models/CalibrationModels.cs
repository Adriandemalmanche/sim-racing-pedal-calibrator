namespace SimRacingPedalCalibrator.Models
{
    public class AxisCalibration
    {
        public int Min { get; set; }
        public int Center { get; set; }
        public int Max { get; set; }
        public int DeadZone { get; set; }

        public AxisCalibration()
        {
            Min = 0;
            Center = 32768;
            Max = 65535;
            DeadZone = 0;
        }

        public AxisCalibration(int min, int center, int max, int deadZone = 0)
        {
            Min = min;
            Center = center;
            Max = max;
            DeadZone = deadZone;
        }

        public double Normalize(int rawValue)
        {
            if (rawValue < Min)
                rawValue = Min;
            if (rawValue > Max)
                rawValue = Max;

            if (rawValue < Center)
            {
                double normalized = (rawValue - Min) / (double)(Center - Min);
                return normalized * 0.5;
            }
            else
            {
                double normalized = (rawValue - Center) / (double)(Max - Center);
                return 0.5 + (normalized * 0.5);
            }
        }
    }

    public class AxisValues
    {
        public int Brake { get; set; }
        public int Throttle { get; set; }
        public int Clutch { get; set; }
    }

    public class CalibrationData
    {
        public AxisCalibration Brake { get; set; } = new(15500, 16700, 18000);
        public AxisCalibration Throttle { get; set; } = new(13980, 18165, 22350);
        public AxisCalibration Clutch { get; set; } = new(14680, 18705, 22730);
    }
}