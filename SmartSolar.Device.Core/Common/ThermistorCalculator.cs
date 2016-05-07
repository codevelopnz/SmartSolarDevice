using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using System.Text;
using System.Threading.Tasks;

namespace SmartSolar.Device.Core.Common
{
	// There are 2 main models used for turning thermistor resistance readings into temperatures
	// per http://www.thinksrs.com/downloads/PDFs/ApplicationNotes/LDC%20Note%204%20NTC%20Calculator.pdf
	// and https://www.thermistor.com/sites/default/files/specsheets/Beta-vs-Steinhart-Hart-Equations.pdf
	public enum ThermistorModel {SteinhartHartModel, BetaModel}

	// These are the parameters that a datasheet for a thermistor will give us, so that we can use the beta model
	// to determine temperature from resistance.
	public class ThermistorBetaModelParameters {
		public double ReferenceResistanceAt25DegC { get; set; }
		public double BetaValue { get; set; }
	}

	// These are the parameters that a datasheet for a thermistor will give us, so that we can use the Steinhart-Hard model
	// to determine temperature from resistance.
	public class ThermistorSteinhartHartModelParameters {
		public double ReferenceResistanceAt25DegC { get; set; }
		public double BetaValue { get; set; }
	}

	/// <summary>
	/// Single responsibility: convert a resistance reading from a thermistor into a temperature reading.
	/// </summary>
	public class ThermistorCalculator
	{
		public double ConvertResistanceToTemperatureCelciusUsingSteinhartHartModel(double resistanceOhms)
		{

			return 0;
		}
		/// <summary>
		/// The Beta model is a simple but not-so-accurate way of calculating a temperature from a thermistor's resistance.
		/// Steinhart-Hart is better, but Beta will have to do if we only know the Beta parameters for our thermistor.
		/// </summary>
		/// <param name="betaModelParameters"></param>
		/// <param name="resistanceOhms"></param>
		/// <returns></returns>
		public double ConvertResistanceToTemperatureCelciusUsingBetaModel(ThermistorBetaModelParameters betaModelParameters, double resistanceOhms)
		{

			//	T = [(1 / β) ln(RT / RT0) + 1 / T0 ] ^ - 1 
			// From: https://www.thermistor.com/sites/default/files/specsheets/Beta-vs-Steinhart-Hart-Equations.pdf
			var celciusToKelvin = 273.15;
			var oneOverBeta = 1/betaModelParameters.BetaValue;
			var logResistanceOverReferenceResistance = Math.Log(resistanceOhms/betaModelParameters.ReferenceResistanceAt25DegC);
			var oneOverReferenceTemp = 1/(25 + celciusToKelvin);

			var resultKelvin = Math.Pow((oneOverBeta * logResistanceOverReferenceResistance) + oneOverReferenceTemp, -1);
			var resultCelcius = resultKelvin - celciusToKelvin;
			return resultCelcius;
		}

	}
}
