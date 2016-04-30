using System.Threading.Tasks;

namespace SmartSolar.Device.Core.Common
{
	public interface IAnalogToDigitalConvertor
	{
		Task Initialise();
		float ReadPinVolts(int pinNumber);
		float ReferenceVoltage { get; set; }
	}
}