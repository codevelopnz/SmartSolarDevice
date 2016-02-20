using Caliburn.Micro;
using SmartSolar.Device.Core.Common;

namespace SmartSolar.Device.Core.Pump
{
	/// <summary>
	/// Single responsibility: control the pump, i.e. tell it to turn on or off depending on the conditions.
	/// </summary>
	public class PumpController: PropertyChangedBase
	{
		private readonly Hardware _hardware;

		public PumpController(Hardware hardware)
		{
			_hardware = hardware;
			// When the output changes state, notify our watchers that our IsPumping has changed
			hardware.PumpOutputConnection.PropertyChanged += (s, e) =>
			{
				if (e.PropertyName == nameof(hardware.PumpOutputConnection.State))
				{
					NotifyOfPropertyChange(nameof(IsPumping));
				}
			};
		}

//		public IOutputConnection Connection
//		{
//			get { return _connection; }
//			set
//			{
//				if (Equals(value, _connection)) return;
//				_connection = value;
//				// When the connection is set, subscribe to the property change, so we can set our IsPumping from it.
//				Connection.PropertyChanged += (s, e) =>
//				{
//					if (e.PropertyName == nameof(Connection.State))
//					{
//						// Tell any of our observers that they may wish to re-get the value of IsPumping, which will pull that value from the connection.state
//						NotifyOfPropertyChange(() => IsPumping);
//					}
//				};
//				// And notify once now, to set the initial value
//				NotifyOfPropertyChange(() => IsPumping);
//				// Also notify that the connection itself has changed, in case anyone cares
//				NotifyOfPropertyChange(() => Connection);
//			}
//		}

		// We expose an IsPumping property, which we take directly from the pumpOutputConnection - just as a convenience for our users.
		public bool? IsPumping
		{
			get { return _hardware.PumpOutputConnection?.State; }
			set { _hardware.PumpOutputConnection.State = value; }
		}
	}
}
