namespace SmartSolar.Device.Core.Common
{
	public interface IAnalogToDigitalConvertor
	{
		void Initialise();
		int ReadPin(int pinNumber);
	}
}