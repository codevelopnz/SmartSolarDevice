using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartSolar.Device.Core.Sensor
{
	public interface ITemperatureReader
	{
		double ReadTemperatureCelcius();
	}
}
