namespace SmartSolar.Device.Core.Common
{
	public class Settings
	{
		// Input pins on ADC
		// 

		// Output pins on GPIO
		public int PumpGpioPin = 22;
		public int PumpLedPin = 17;
		public int ElementGpioPin = 27;
		public int ElementLedPin = 18;

		// How warm would we like the temperature to get from electricity heating?
		public int ElectricityTargetDegC = 55;

//		public int HoldOffTemperature = 40;
//		public int HoldOffMinutes = 480;

		public int FrostDegC = 4;

		// The temperature either side of a target temperature at which we'll turn things on and off 
		// - to avoid "thrashing" where we turn a pump or element on/off repeatedly as the measured values varies just slightly either side of the target value.
		public int HysteresisFactorDegC = 1;



		public int PumpOffTemperatureDifference = 2;
			// these are normally higher....but for testing with just body temp on the thermisters

		public int PumpOnTemperatureDifference = 5;
			// these are normally higher....but for testing with just body temp on the thermisters

		// How warm would we like the temperature to get from solar heating?
		public int SolarTargetDegC = 70;
	}
}