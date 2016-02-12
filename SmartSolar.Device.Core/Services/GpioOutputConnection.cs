using Windows.Devices.Gpio;
using Caliburn.Micro;
using SmartSolar.Device.Core.Domain;

namespace SmartSolar.Device.Core.Services
{
	/// <summary>
	/// Single responsibility: represent a physical connection to a pump, via a GPIO pin
	/// </summary>
	public class GpioOutputConnection : PropertyChangedBase, IOutputConnection
	{
		private GpioPin _gpioPin;
		private bool? _state = null;

		public bool? State
		{
			get { return _state; } 
			set
			{
				if (value == _state) return;
				_state = value;
				NotifyOfPropertyChange(() => State);
			}
		}

		public void Configure(GpioPin gpioPin)
		{
			_gpioPin = gpioPin;
		}
	}
}