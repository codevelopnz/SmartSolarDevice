using System.Collections;

namespace SmartSolar.Device.Core.Domain
{
	public interface IAnalogToDigitalConvertor
	{
		void Initialise();
		BitArray ReadPin(int pinNumber);
	}
}