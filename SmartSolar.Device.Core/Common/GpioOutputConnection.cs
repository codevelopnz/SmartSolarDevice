using Windows.Devices.Gpio;
using Caliburn.Micro;

namespace SmartSolar.Device.Core.Common
{
	/// <summary>
	///     Single responsibility: represent a physical output connection to something, via a GPIO pin
	/// </summary>
	public class GpioOutputConnection : PropertyChangedBase, IOutputConnection
	{
		private GpioPin _gpioPin;
		private bool? _state;

		public bool? State
		{
			get { return _state; }
			set
			{
				if (value == _state) return;
				_state = value;
				_gpioPin.Write(value == true ? GpioPinValue.Low : GpioPinValue.High);
				NotifyOfPropertyChange(() => State);
			}
		}

		public void Configure(GpioPin gpioPin)
		{
			_gpioPin = gpioPin;
            // init the pin
            _gpioPin.SetDriveMode(GpioPinDriveMode.Output);
            _gpioPin.Write(GpioPinValue.High);
            _gpioPin.Write(GpioPinValue.Low);
            _gpioPin.Write(GpioPinValue.High);
        }
	}
}