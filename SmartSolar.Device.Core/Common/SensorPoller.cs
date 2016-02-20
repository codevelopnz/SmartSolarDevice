using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Caliburn.Micro;
using SmartSolar.Device.Core.Messages;

namespace SmartSolar.Device.Core.Common
{
	/// <summary>
	/// Single responsibility: poll the sensors on a regular basis, and tell the app afterwards
	/// </summary>
	public class SensorPoller
	{
		private readonly Hardware _hardware;
		private readonly IEventAggregator _eventAggregator;

		public SensorPoller(Hardware hardware, IEventAggregator eventAggregator)
		{
			_hardware = hardware;
			_eventAggregator = eventAggregator;
		}

		private void pollOnce()
		{
			_hardware.RoofTemperatureReader.ReadTemperatureDegC();
			_hardware.InletTemperatureReader.ReadTemperatureDegC();
			_hardware.TankTemperatureReader.ReadTemperatureDegC();
		}

		public void PollContinuously()
		{
			new TaskFactory().StartNew(async () =>
			{
				pollOnce();
				_eventAggregator.PublishOnUIThread(new SensorsWereRead());
				await Task.Delay(TimeSpan.FromSeconds(1));
			});

		}
	}
}
