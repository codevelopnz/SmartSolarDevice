using System;
using System.Threading.Tasks;
using Windows.Devices.Enumeration;
using Windows.Devices.Spi;

namespace SmartSolar.Device.Core.Common
{
	/// <summary>
	///     Single responsibility: interface with an MCP3008 ADC chip over the SPI interface.
	/// </summary>
	public class Mcp3008 : IAnalogToDigitalConvertor
	{
		private SpiDevice _spiDevice;

		// For a great writeup on what these buffers mean, see:
		// http://blog.falafel.com/mcp3008-analog-to-digital-conversion/

		// TODO: what do these magic numbers mean?
		private readonly byte[] _writeBuffer = new byte[3];
		private readonly byte[] _readBuffer = new byte[3];
			//00000110 00; /* It is SPI port serial input pin, and is used to load channel configuration data into the device*/
			//00000110 00; /* It is SPI port serial input pin, and is used to load channel configuration data into the device*/

		public Mcp3008()
		{
			// Set the default connection parameters for this chip. 
			// - connect via SPI0
			SpiControllerName = "SPI0";
			// - line 0 maps to physical pin number 24 on the Rpi2
			SpiChipSelectLine = 0;
		}

		// Configurable parameters - should be set before Initialise() is called
		public string SpiControllerName { get; set; }
		public int SpiChipSelectLine { get; set; }
		public float ReferenceVoltage { get; set; }

		public async Task Initialise()
		{
			var settings = new SpiConnectionSettings(SpiChipSelectLine)
			{
//				ClockFrequency = 500000,
				ClockFrequency = 3600000, // 3.6MHz
				Mode = SpiMode.Mode0
			};

			var spiAqs = SpiDevice.GetDeviceSelector(SpiControllerName);
			var deviceInfo = await DeviceInformation.FindAllAsync(spiAqs);
			_spiDevice = await SpiDevice.FromIdAsync(deviceInfo[0].Id, settings);
		}

		public float ReadPinVolts(int pinNumber)
		{
			if (_spiDevice == null)
			{
				throw new Exception("SPI device not initialised");
			}

			var channel = PinToChannel(pinNumber);

			// See http://blog.falafel.com/mcp3008-analog-to-digital-conversion/ for details of this dark, dark magic
			// - first byte is start bit
			_writeBuffer[0] = 0x01; 
			// - second byte tells the ADC what we want to read
			_writeBuffer[1] = GetConfigurationByteToReadChannel(channel); 
//			_writeBuffer[1] = 0x80;
			// - third byte isn't used AFAIK
			_writeBuffer[2] = 0x0;

			_spiDevice.TransferFullDuplex(_writeBuffer, _readBuffer);
			var result = ConvertBytesToInt(_readBuffer);
			return (result / 1023f) * ReferenceVoltage; // 1023 because it's a 10-bit ADC, and 2^10 = 1024
		}

		private static int ConvertBytesToInt(byte[] bytes)
		{
			// From http://blog.falafel.com/mcp3008-analog-to-digital-conversion/
			// - first byte returned is 0 (00000000), 
			// - second byte returned we are only interested in the last 2 bits 00000011 (mask of &3) 
			// - then shift result 8 bits to make room for the data from the 3rd byte (makes 10 bits total)
			// - third byte, need all bits, simply add it to the above result 
			var result = bytes[1] & 0x03;
			result <<= 8;
			result += bytes[2];
			return result;
		}

		private static int PinToChannel(int pin) {
			// Pin 1 is Channel 0, etc up to Pin 8 is Channel 7
			// http://blog.falafel.com/mcp3008-analog-to-digital-conversion/
			if (pin < 1 || pin > 8) throw new ArgumentException("MCP3008 doesn't have an input channel on Pin " + pin);
			return pin - 1;
		}

		private static byte GetConfigurationByteToReadChannel(int channel)
		{
			// Embodies the table 5.2 from http://blog.falafel.com/mcp3008-analog-to-digital-conversion/
			// (At least, the single-ended reads - we don't need to read the differential between two channels in this project)
			byte leftBit = 0x1; // would be 0x0 for differential
			byte channelSelectionBits = (byte) channel;
//			byte differentialBit = (byte) 0x0 << 8;


			var result = (byte) (
				// left-most bit, shifted to left-most position
				leftBit << 7
				// next 3 bytes are the channel
				| (byte)channelSelectionBits << 4
				// last 4 bytes aren't needed, they'll be zero here)
			);

			return result;
		}

	}
}