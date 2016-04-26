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
		private readonly byte[] _readBuffer = new byte[3] {0x06, 0x00, 0x00};
			//00000110 00; /* It is SPI port serial input pin, and is used to load channel configuration data into the device*/


		private SpiDevice _spiDevice;
		// TODO: what do these magic numbers mean?
		private readonly byte[] _writeBuffer = new byte[3] {0x06, 0x00, 0x00};
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

		public async Task Initialise()
		{
			var settings = new SpiConnectionSettings(SpiChipSelectLine)
			{
				ClockFrequency = 500000,
				Mode = SpiMode.Mode0
			};

			var spiAqs = SpiDevice.GetDeviceSelector(SpiControllerName);
			var deviceInfo = await DeviceInformation.FindAllAsync(spiAqs);
			_spiDevice = await SpiDevice.FromIdAsync(deviceInfo[0].Id, settings);
		}

		public int ReadPin(int pinNumber)
		{
			if (_spiDevice == null)
			{
				throw new Exception("SPI device not initialised");
			}

			// TODO: how do we set the writeBuffer from the pin? Paste a URL with some details of this chip in here.
			_writeBuffer[1] = 0x40;
			_spiDevice.TransferFullDuplex(_writeBuffer, _readBuffer);
			return ConvertBytesToInt(_readBuffer);
		}

		private static int ConvertBytesToInt(byte[] bytes)
		{
			var result = bytes[1] & 0x0F;
			result <<= 8;
			result += bytes[2];
			return result;
		}
	}
}