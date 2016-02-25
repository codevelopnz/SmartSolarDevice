using SmartSolar.Device.Core.Common;

namespace SmartSolar.Device.Core.Pump
{
	/// <summary>
	/// Single responsibility: hold all the factors that influence whether the pump should be on or off.
	/// These could be just function parameters to the PumpStrategy below - but then you end up with a lot of meaningless
	/// boolean parameters, so better to have a class encapsulating them.
	/// </summary>
	public class PumpStrategyParams
	{
		public bool IsPumpCurrentlyOn { get; set; }
		public double RoofDegC { get; set; }
		public double InletDegC { get; set; }

	}

	/// <summary>
	/// Single responsibility: to make decisions about whether the pump should currently be on or off.
	/// </summary>
	public class PumpStrategy
	{
		private Settings _settings;

		public PumpStrategy(Settings settings)
		{
			_settings = settings;
		}

		// should we be switching on the pump (using electricity) at this point?
		// a big assumption with putting this logic here is that both the pump and element are 
		// allowed to be on at the same time.  My thinking is that this should be possible in the
		// case of a frost condition or even when 'boosting' and solar is hot and ready to go.
		public bool ShouldPumpBeOn(PumpStrategyParams @params)
		{
			var shouldStartFrostPumping =
				@params.RoofDegC < _settings.FrostDegC;

			var shouldContinueFrostPumping = 
				@params.IsPumpCurrentlyOn && 
				(@params.RoofDegC < _settings.FrostDegC + _settings.HysteresisFactorDegC);

			var isTankBelowSolarTarget = (@params.InletDegC <= _settings.SolarTargetDegC);
			var isWorthStartingPump = (@params.RoofDegC > @params.InletDegC + _settings.PumpOnTemperatureDifference);
			var shouldStartPumpingHotterRoofWater = !@params.IsPumpCurrentlyOn && isTankBelowSolarTarget && isWorthStartingPump;
			
			var isWorthContinuingPump = (@params.RoofDegC > @params.InletDegC + _settings.PumpOffTemperatureDifference);
			var shouldContinuePumpingHotterRoofWater = @params.IsPumpCurrentlyOn && isWorthContinuingPump && isTankBelowSolarTarget;

			return (shouldStartFrostPumping || shouldContinueFrostPumping || shouldStartPumpingHotterRoofWater || shouldContinuePumpingHotterRoofWater);
		}
	}
}