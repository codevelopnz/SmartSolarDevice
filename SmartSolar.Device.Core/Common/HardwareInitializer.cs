using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SmartSolar.Device.Core.Sensor;

namespace SmartSolar.Device.Core.Common
{
	/// <summary>
	/// Single responsibility: to do the one-off initialization of hardware.
	/// The reason this is in a separate class, is that the initialization is async, and therefore
	/// needs to be done off the main thread - when this was done in App.xaml.cs, we got a deadlock.
	/// http://blog.stephencleary.com/2012/07/dont-block-on-async-code.html
	/// So, it was encapsulated in this class & called from the first view that loads
	/// where it can happily run async, e.g. the MainPageView - looks like Caliburn Micro
	/// is OK with async in e.g. OnInitialize()
	/// http://stackoverflow.com/questions/15417354/will-caliburn-micro-do-the-right-thing-with-async-method-on-viewmodel

	/// </summary>
	public class HardwareInitializer
	{
		private bool _hasInitialized;
		private readonly IAnalogToDigitalConvertor _adc;
		private readonly SensorPoller _sensorPoller;
		private readonly Hardware _hardware;

		public HardwareInitializer(IAnalogToDigitalConvertor adc, SensorPoller sensorPoller, Hardware hardware)
		{
			_adc = adc;
			_sensorPoller = sensorPoller;
			_hardware = hardware;
		}

		public async void Initialize()
		{
			// Only do this once
			if (_hasInitialized) return;
			_hasInitialized = true;

			// Initialize the ADC
			await _adc.Initialise();

            // Set the initial states of the outputs
            _hardware.ElementOutputConnection.State = false;
            _hardware.ElementLedOutputConnection.State = false;
            _hardware.PumpOutputConnection.State = false;
            _hardware.PumpLedOutputConnection.State = false;

            // Kick off the poller
            _sensorPoller.PollContinuously();
		}
	}
}
