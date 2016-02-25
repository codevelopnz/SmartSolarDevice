using Caliburn.Micro;

namespace SmartSolar.Device.Core.Sensor
{
	public class FakeTemperatureReader: PropertyChangedBase, ITemperatureReader
	{
		private double? _lastTemperatureDegC;

		public FakeTemperatureReader()
		{
		}

		public double? FakeTemperatureDegC { get; set; }

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
			LastTemperatureDegC = FakeTemperatureDegC ?? 0;

			return LastTemperatureDegC.Value;
		}
	}
}
