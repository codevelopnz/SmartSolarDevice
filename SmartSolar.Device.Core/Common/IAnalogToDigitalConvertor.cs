using System.Threading.Tasks;

namespace SmartSolar.Device.Core.Common
{
	public interface IAnalogToDigitalConvertor
	{
		Task Initialise();
		int ReadPin(int pinNumber);
	}
}