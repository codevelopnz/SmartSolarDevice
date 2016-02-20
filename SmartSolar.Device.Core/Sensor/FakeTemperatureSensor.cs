namespace SmartSolar.Device.Core.Sensor
{
	public class FakeTemperatureReader: ITemperatureReader
	{
		public int FakeTemperatureDegC { get; set; }
		public double ReadTemperatureCelcius()
		{
			return FakeTemperatureDegC;
		}
	}
}
