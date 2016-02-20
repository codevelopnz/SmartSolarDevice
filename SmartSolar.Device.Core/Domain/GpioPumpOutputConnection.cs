using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SmartSolar.Device.Core.Services;

namespace SmartSolar.Device.Core.Domain
{
	public class GpioPumpOutputConnection: GpioOutputConnection , IPumpOutputConnection 
	{
	}
}
