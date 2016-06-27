using Caliburn.Micro;
using SmartSolar.Device.Core.Common;
using SmartSolar.Device.Core.Messages;

namespace SmartSolar.Device.Core.Pump
{
	/// <summary>
	/// Single responsibility: control the pump, i.e. tell it to turn on or off depending on the conditions.
	/// </summary>
	public class PumpController
		: PropertyChangedBase
		, IHandle<SensorsWereRead>
	{
		private readonly IEventAggregator _eventAggregator;
		private readonly Hardware _hardware;
		private readonly PumpStrategy _pumpStrategy;
		private readonly PumpStrategyParams _pumpStrategyParams;

		public PumpController(
			IEventAggregator eventAggregator,
			Hardware hardware,
			PumpStrategy pumpStrategy,
			PumpStrategyParams pumpStrategyParams
		)
		{
			_eventAggregator = eventAggregator;
			_eventAggregator.Subscribe(this);
			_hardware = hardware;
			_pumpStrategy = pumpStrategy;
			_pumpStrategyParams = pumpStrategyParams;
		}

		public void Handle(SensorsWereRead message)
		{
			_pumpStrategyParams.RoofDegC = _hardware.RoofTemperatureReader.LastTemperatureDegC.Value;
			_pumpStrategyParams.InletDegC = _hardware.InletTemperatureReader.LastTemperatureDegC.Value;
			_pumpStrategyParams.IsPumpCurrentlyOn = _hardware.PumpOutputConnection.State.Value;

			var shouldPumpBeOn = _pumpStrategy.ShouldPumpBeOn(_pumpStrategyParams);

			_hardware.PumpOutputConnection.State = shouldPumpBeOn;
            _hardware.PumpLedOutputConnection.State = shouldPumpBeOn;
        }
	}
}
