using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SmartSolar.Device.Core.Sensor;

namespace SmartSolar.Device.Core.Common
{
	/// <summary>
	/// Single responsibility: represent the hardware configuration of this system,
	/// or fakes if we're running without hardware.
	/// </summary>
	public class Hardware
	{
        public IOutputConnection PumpOutputConnection { get; set; }
        public IOutputConnection PumpLedOutputConnection { get; set; }
        public IOutputConnection ElementOutputConnection { get; set; }
        public IOutputConnection ElementLedOutputConnection { get; set; }
        public ITemperatureReader TankTemperatureReader { get; set; }
		public ITemperatureReader InletTemperatureReader { get; set; }
		public ITemperatureReader RoofTemperatureReader { get; set; }

		public Hardware(
            IOutputConnection pumpOutputConnection,
            IOutputConnection pumpLedOutputConnection,
            IOutputConnection elementOutputConnection,
            IOutputConnection elementLedOutputConnection,
            ITemperatureReader tankTemperatureReader,
			ITemperatureReader inletTemperatureReader,
			ITemperatureReader roofTemperatureReader

			)
		{
            PumpOutputConnection = pumpOutputConnection;
            PumpLedOutputConnection = pumpLedOutputConnection;
            ElementOutputConnection = elementOutputConnection;
            ElementLedOutputConnection = elementLedOutputConnection;
            TankTemperatureReader = tankTemperatureReader;
			InletTemperatureReader = inletTemperatureReader;
			RoofTemperatureReader = roofTemperatureReader;
		}
	}
}
