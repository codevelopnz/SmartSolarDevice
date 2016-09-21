using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartSolar.Device.Core.Common
{
    public class FakeAnalogToDigitalConvertor : IAnalogToDigitalConvertor
    {
        public float ReferenceVoltage
        {
            get { return 0; }
            set { }
        }

        public Task Initialise()
        {
            return Task.FromResult(true);
        }

        public float ReadPinVolts(int pinNumber)
        {
            return 0;
        }
    }
}
