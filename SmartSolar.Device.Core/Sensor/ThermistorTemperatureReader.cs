using System;
using System.Runtime.InteropServices.WindowsRuntime;
using Caliburn.Micro;
using SmartSolar.Device.Core.Common;

namespace SmartSolar.Device.Core.Sensor
{
	/// <summary>
	/// Single responsibility: read the current temperature of a thermistor connected via an ADC.
	/// </summary>
	public class ThermistorTemperatureReader: PropertyChangedBase, ITemperatureReader
	{
		private IAnalogToDigitalConvertor _adc;
		private readonly Settings _settings;
		private readonly ThermistorCalculator _thermistorCalculator;
		private double? _lastTemperatureDegC;

		public ThermistorTemperatureReader(IAnalogToDigitalConvertor adc, Settings settings, ThermistorCalculator thermistorCalculator)
		{
			_adc = adc;
			_settings = settings;
			_thermistorCalculator = thermistorCalculator;
			adc.ReferenceVoltage = settings.AdcReferenceVoltage;
		}

		// These two are set during configuration
		public int PinNumber { get; set; }
		public ThermistorModelParameters ThermistorModelParameters { get; set; }

		public double? LastTemperatureDegC
		{
			get { return _lastTemperatureDegC; }
			set
			{
				if (value.Equals(_lastTemperatureDegC)) return;
				_lastTemperatureDegC = value;
				NotifyOfPropertyChange(() => LastTemperatureDegC);
			}
		}

		public double ReadTemperatureDegC()
		{
			var pinVolts = _adc.ReadPinVolts(PinNumber);
			LastTemperatureDegC = ConvertPinVoltsToDegreesCelcius(pinVolts);
			return LastTemperatureDegC.Value;
		}

		public double ConvertPinVoltsToDegreesCelcius(float pinVolts)
		{
			// We know that our thermistor is in series with a 10k resistor. 
			// Think of the thermistor on top of the 10k resistor, with the measurement point in between.
			// The current through both of them must therefore be equal.
			// Definitions:
			// * Vref = reference voltage, at the top of both resistors
			// * Vmeasured = measured voltage, in between the two resistors
			// * Rref = our 10k resistor
			// * Rthermistor = resistance of our thermistor
			// Rmeasured = measured resistance of the thermistor
			// Basic ohms' law says V=IR, so I = V/R, so taking this for each resistor
			// 1. For the thermistor, on top of the 10k resistor:
			//    I = (Vref - Vmeasured) / Rthermistor
			// 2. For the 10k resistor:
			//    I = Vmeasured / 10k
			// ... since we know those two currents are equal:
			// (Vref - Vmeasured) / Rthermistor = Vmeasured / 10k
			// ... rearranging because we want to know Rthermistor
			// (Vref - Vmeasured) * 10k / Vmeasured = Rthermistor

			var thermistorResistance = (_settings.AdcReferenceVoltage - pinVolts) * 10000 / pinVolts;

			var result = _thermistorCalculator.ConvertResistanceToTemperatureCelcius(ThermistorModelParameters, thermistorResistance);
			return result;
		}
	}
}
