using System;
using FluentAssertions;
using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
using SmartSolar.Device.Core.Domain;
using SmartSolar.Device.Core.Services;

namespace SmartSolar.Device.Test
{
	[TestClass]
	public class ElementStrategyTests
	{
		private ElementStrategy _elementStrategy;
		private Settings _settings;

		public ElementStrategyTests()
		{
			_settings = new Settings();
			_elementStrategy = new ElementStrategy(_settings);
		}

		[TestMethod]
		public void TurnsOnBasedOnElementTemp()
		{

			var lowerTarget = _settings.ElectricityTarget - _settings.HysteresisFactorDegrees;
			var upperTarget = _settings.ElectricityTarget + _settings.HysteresisFactorDegrees;

			_elementStrategy.ShouldElementBeOn(
				new ElementStrategyParams
				{
					InletTemperature = lowerTarget - 1,
					IsElementCurrentlyOn = false
				}
			).Should().BeTrue("because the current inlet temp is less than the electricity target (less hysteresis factor)");

			_elementStrategy.ShouldElementBeOn(
				new ElementStrategyParams
				{
					InletTemperature = lowerTarget,
					IsElementCurrentlyOn = false
				}
			).Should().BeTrue("because the current inlet temp is the same as than the electricity target (less hysteresis factor)");

			_elementStrategy.ShouldElementBeOn(
				new ElementStrategyParams
				{
					InletTemperature = (lowerTarget + upperTarget) / 2,
					IsElementCurrentlyOn = true
				}
			).Should().BeTrue("because the current inlet temp is in the target range, and we're already on, so keep going!");

			_elementStrategy.ShouldElementBeOn(
				new ElementStrategyParams
				{
					InletTemperature = upperTarget + 1,
					IsElementCurrentlyOn = true
				}
			).Should().BeFalse("because we turn off after we've exceeded the top of the target temp range (including hysteresis)");

			_elementStrategy.ShouldElementBeOn(
				new ElementStrategyParams
				{
					InletTemperature = (lowerTarget + upperTarget) / 2,
					IsElementCurrentlyOn = false
				}
			).Should().BeFalse("because the current inlet temp is in the target range, but we're not already on, so we must be in the cooldown part of the hysteresis");

		}
	}
}
