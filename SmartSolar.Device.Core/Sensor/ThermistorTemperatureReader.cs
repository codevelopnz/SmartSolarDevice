using System;
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
		private double? _lastTemperatureDegC;

		public ThermistorTemperatureReader(IAnalogToDigitalConvertor adc, Settings settings)
		{
			_adc = adc;
			_settings = settings;
			adc.ReferenceVoltage = settings.AdcReferenceVoltage;
		}

		public int PinNumber { get; set; }

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

			//var volts = Convert.ToDecimal(res) * (5M / 4095.0M);
			////var ohms = ((1.0M / volts) * 5000M) + 10000M;
			////var ohms = volts / 0.0001M;
			//var ohms = ((5M / volts) - 1M) * 10000M;
			//var lnohm = Math.Log(Convert.ToDouble(ohms), Math.E);

			//// a, b, & c values from http://www.thermistor.com/calculators.php
			//// using curve R (-6.2%/C @ 25C) Mil Ratio X
			//var a = 0.001821776240470;
			//var b = 0.000159566897583;
			//var c = 0.000000080342408;

			//// Steinhart Hart Equation
			//// T = 1/(a + b[ln(ohm)] + c[ln(ohm)]^3)

			//var t1 = (b * lnohm); // b[ln(ohm)]
			//var c2 = c * lnohm; // c[ln(ohm)]
			//var t2 = Math.Pow(c2, 3); // c[ln(ohm)]^3
			//var temp = 1 / (a + t1 + t2); //calcualte temperature
			//var tempc = temp - 273.15 - 4; // K to C
			//return tempc;

			double thermisterNominal = 10000;
			double temperatureNominal = 25;
			double bCoefficient = 3369;
			double steinhart;

//			var volts = Convert.ToDouble(adcReading) * (3.3 / 4095.0);
// Trevor: why do we -1 here? How are we converting from volts on the pin, to ohms?
			var ohms = ((_settings.AdcReferenceVoltage / pinVolts) - 1.0) * 10000.0;

			steinhart = ohms / thermisterNominal;     // (R/Ro)
			steinhart = Math.Log(steinhart);                  // ln(R/Ro)
			steinhart /= bCoefficient;                   // 1/B * ln(R/Ro)
			steinhart += 1.0 / (temperatureNominal + 273.15); // + (1/To)
			steinhart = 1.0 / steinhart;                 // Invert
			steinhart -= 273.15;                         // convert to C

			return steinhart;
		}
	}
}
