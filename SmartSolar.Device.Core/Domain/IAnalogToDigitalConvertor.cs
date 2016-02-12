using System.Collections;

namespace SmartSolar.Device.Core.Domain
{
	public interface IAnalogToDigitalConvertor
	{
		void Initialise();
		int ReadPin(int pinNumber);
	}
}