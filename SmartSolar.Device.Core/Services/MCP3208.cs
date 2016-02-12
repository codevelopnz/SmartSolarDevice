using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Spi;
using Windows.Devices.Enumeration;
using SmartSolar.Device.Core.Domain;

namespace SmartSolar.Device.Core.Services
{
	/// <summary>
	/// Single responsibility: interface with an MCP3208 ADC chip over the SPI interface.
	/// </summary>
	public class Mcp3208 : IAnalogToDigitalConvertor
	{
		// Configurable parameters - should be set before Initialise() is called
		public string SpiControllerName { get; set; }
		public Int32 SpiChipSelectLine { get; set; }


		private SpiDevice _spiDevice;
		// TODO: what do these magic numbers mean?
		private byte[] _writeBuffer = new byte[3] { 0x06, 0x00, 0x00 };//00000110 00; /* It is SPI port serial input pin, and is used to load channel configuration data into the device*/
		private byte[] _readBuffer = new byte[3] { 0x06, 0x00, 0x00 };//00000110 00; /* It is SPI port serial input pin, and is used to load channel configuration data into the device*/
		public Mcp3208()
		{
			// Set the default connection parameters for this chip. 
			// - connect via SPI0
			SpiControllerName = "SPI0";
			// - line 0 maps to physical pin number 24 on the Rpi2
			SpiChipSelectLine = 0;
		}

		public async void Initialise()
		{
			var settings = new SpiConnectionSettings(SpiChipSelectLine)
			{
				ClockFrequency = 500000,
				Mode = SpiMode.Mode0
			};

			string spiAqs = SpiDevice.GetDeviceSelector(SpiControllerName);
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
			int result = bytes[1] & 0x0F;
			result <<= 8;
			result += bytes[2];
			return result;
		}
	}
}
