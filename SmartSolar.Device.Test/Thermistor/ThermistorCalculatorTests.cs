using FluentAssertions;
using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
using SmartSolar.Device.Core.Common;
using SmartSolar.Device.Core.Element;

namespace SmartSolar.Device.Test.Element
{
	[TestClass]
	public class ThermistorCalculatorTests
	{
		private ThermistorCalculator _thermistorCalculator;

		public ThermistorCalculatorTests()
		{
			_thermistorCalculator = new ThermistorCalculator();
		}

		[TestMethod]
		public void GivenBetaModelParameters_WhenIHaveAResistance_ThenICanCalculateTemperature()
		{
			// Arrange
			// Values for my thermistor, from http://www.surplustronics.co.nz/products/2140-ntc-thermistor-10k-ohms
			var betaModelParameters = new ThermistorBetaModelParameters
			{
				BetaValue = 4100,
				ReferenceResistanceAt25DegC = 10000
			};


			// Act/Assert
			_thermistorCalculator.ConvertResistanceToTemperatureCelciusUsingBetaModel(
				betaModelParameters, 
				10000).Should().BeApproximately(25, 0.01, "because that's the reference temperature");

			_thermistorCalculator.ConvertResistanceToTemperatureCelciusUsingBetaModel(
				betaModelParameters, 
				1000).Should().BeApproximately(84.96, 0.01, "because that's the the value I get from the online calculator at http://www.thinksrs.com/downloads/programs/Therm%20Calc/NTCCalibrator/NTCcalculator.htm");

			_thermistorCalculator.ConvertResistanceToTemperatureCelciusUsingBetaModel(
				betaModelParameters, 
				28000).Should().BeApproximately(4.23, 0.01, "because that's the the value I get from the online calculator at http://www.thinksrs.com/downloads/programs/Therm%20Calc/NTCCalibrator/NTCcalculator.htm");

		}
	}
}
