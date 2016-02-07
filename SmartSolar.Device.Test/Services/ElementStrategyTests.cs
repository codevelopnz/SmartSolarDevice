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
			_elementStrategy = new ElementStrategy();
			_settings = new Settings();
		}

		[TestMethod]
		public void TurnsOnBasedOnElementTemp()
		{

			_elementStrategy.ShouldElementBeOn(
				new ElementStrategyParams
				{
					IsElementOn = false,
					InletTemperature = _settings.ElectricityTarget - 1,
					IsBoosting = false
				}, _settings
			).Should().BeTrue("because the current inlet temp is less than the electricity target");

			_elementStrategy.ShouldElementBeOn(
				new ElementStrategyParams
				{
					IsElementOn = false,
					InletTemperature = _settings.ElectricityTarget,
					IsBoosting = false
				}, _settings
			).Should().BeTrue("because the current inlet temp is the same as than the electricity target");

			_elementStrategy.ShouldElementBeOn(
				new ElementStrategyParams
				{
					IsElementOn = false,
					InletTemperature = _settings.ElectricityTarget + 1,
					IsBoosting = false
				}, _settings)
			.Should().BeFalse("because the current inlet temp is higher than the electricity target, and we're not boosting");

			_elementStrategy.ShouldElementBeOn(
				new ElementStrategyParams
				{
					IsElementOn = true,
					InletTemperature = _settings.ElectricityTarget + 1,
					IsBoosting = false
				}, _settings)
			.Should().BeTrue("because although the current inlet temp is above the target, we're boosting");
		}
	}
}
