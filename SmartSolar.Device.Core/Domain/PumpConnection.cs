using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Caliburn.Micro;

namespace SmartSolar.Device.Core.Domain
{
	public class Pump: PropertyChangedBase
	{
		private bool? _isPumping;
		private IOutputConnection _connection;

		public IOutputConnection Connection
		{
			get { return _connection; }
			set
			{
				if (Equals(value, _connection)) return;
				_connection = value;
				// When the connection is set, subscribe to the property change, so we can set our IsPumping from it.
				Connection.PropertyChanged += (s, e) =>
				{
					if (e.PropertyName == nameof(Connection.State))
					{
						// Tell any of our observers that they may wish to re-get the value of IsPumping, which will pull that value from the connection.state
						NotifyOfPropertyChange(() => IsPumping);
					}
				};
				// And notify once now, to set the initial value
				NotifyOfPropertyChange(() => IsPumping);
				// Also notify that the connection itself has changed, in case anyone cares
				NotifyOfPropertyChange(() => Connection);
			}
		}

		// We expose an IsPumping property, which we take directly from the Connection - just as a convenience for our users.
		public bool? IsPumping
		{
			get { return Connection?.State; }
			set
			{
				if (value == _isPumping) return;
				_isPumping = value;
				NotifyOfPropertyChange(() => IsPumping);
			}
		}
	}
}
