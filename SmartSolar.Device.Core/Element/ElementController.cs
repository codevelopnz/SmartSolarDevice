using Caliburn.Micro;
using SmartSolar.Device.Core.Common;
using SmartSolar.Device.Core.Messages;
using SmartSolar.Device.Core.Pump;

namespace SmartSolar.Device.Core.Element
{
	/// <summary>
	/// Single responsibility: control the pump, i.e. tell it to turn on or off depending on the conditions.
	/// </summary>
	public class ElementController
		: PropertyChangedBase
		, IHandle<SensorsWereRead>
	{
		private readonly Hardware _hardware;
		private readonly ElementStrategy _elementStrategy;
		private readonly ElementStrategyParams _elementStrategyParams;

		public ElementController(
			IEventAggregator eventAggregator,
			Hardware hardware,
			ElementStrategy elementStrategy,
			ElementStrategyParams elementStrategyParams
			)
		{
			eventAggregator.Subscribe(this);
			_hardware = hardware;
			_elementStrategy = elementStrategy;
			_elementStrategyParams = elementStrategyParams;
		}


		public void Handle(SensorsWereRead message)
		{
			_elementStrategyParams.InletTemperature = _hardware.InletTemperatureReader.LastTemperatureDegC.Value;
			_elementStrategyParams.IsElementCurrentlyOn = _hardware.ElementOutputConnection.State.Value;

			var shouldElementBeOn = _elementStrategy.ShouldElementBeOn(_elementStrategyParams);

			_hardware.ElementOutputConnection.State = shouldElementBeOn;
            _hardware.ElementLedOutputConnection.State = shouldElementBeOn;
        }
	}
}