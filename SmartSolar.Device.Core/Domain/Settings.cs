using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartSolar.Device.Core.Domain
{
	public class Settings
	{
		public int PumpOnTemperatureDifference = 5; // these are normally higher....but for testing with just body temp on the thermisters
		public int PumpOffTemperatureDifference = 2; // these are normally higher....but for testing with just body temp on the thermisters

		// How warm would we like the temperature to get from electricity heating?
		public int ElectricityTargetDegC = 55;
		// How warm would we like the temperature to get from solar heating?
		public int SolarTargetDegC = 70;

		// The temperature either side of a target temperature at which we'll turn things on and off 
		// - to avoid "thrashing" where we turn a pump or element on/off repeatedly as the measured values varies just slightly either side of the target value.
		public int HysteresisFactorDegC = 1;

//		public int HoldOffTemperature = 40;
//		public int HoldOffMinutes = 480;

		public int FrostDegC = 4;


		public int PumpGpioPin = 22;
		public int ElementGpioPin = 27;
		public int PumpLedPin = 17;
		public int ElementLedPin = 18;
	}
}
