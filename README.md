# Sim Racing Pedal Calibrator

A modern Windows 11 calibration application for sim racing pedals. Features real-time raw value reading, interactive calibration, and persistent registry storage.

## Features

- 📊 **Live Graphing** - Visualize pedal values in real-time
- 🎛️ **Interactive Calibration** - Press pedals to auto-detect min/max/center values
- 💾 **Registry Integration** - Load and save calibration data to Windows Registry
- 🎨 **Modern UI** - Built with WinUI 3 for a native Windows 11 experience
- ⌨️ **Dead Zone Control** - Fine-tune dead zones for each axis

## Supported Devices

Currently configured for:
- VID: 0x16C0
- PID: 0x05DF
- 3 Axes: Brake (X), Throttle (Y), Clutch (Z)

## Technology Stack

- **Framework**: .NET 8 with WinUI 3
- **Device Communication**: SharpDX DirectInput
- **Graphing**: LiveCharts2
- **Registry Access**: Native .NET Registry APIs

## Getting Started

### Prerequisites

- Windows 11
- .NET 8 SDK
- Visual Studio 2022 or higher (recommended)

### Installation

1. Clone the repository
2. Open `SimRacingPedalCalibrator.sln` in Visual Studio
3. Restore NuGet packages
4. Build and run

## Usage

1. Launch the application
2. Current calibration values load automatically from Registry
3. Press each pedal fully to calibrate (min and max)
4. Adjust dead zones as needed
5. Click "Save Calibration" to update Registry values

## Registry Format

Calibration data is stored as hex values (3 × 4-byte little-endian integers):
```
[HKEY_CURRENT_USER\System\CurrentControlSet\Control\MediaProperties\PrivateProperties\DirectInput\VID_16C0&PID_05DF\Calibration\0\Type\Axes\{axis}]
"Calibration"=hex:{min},{center},{max}
```

## License

MIT

## Contributing

Feel free to submit issues and enhancement requests!