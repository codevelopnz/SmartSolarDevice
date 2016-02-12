using Caliburn.Micro;
using SmartSolar.Device.Core.Domain;

namespace SmartSolar.Device.Core.Services
{
	public class FakeOutputConnection : PropertyChangedBase, IOutputConnection
	{

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
	}
}