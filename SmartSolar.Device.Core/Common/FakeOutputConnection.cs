using Caliburn.Micro;

namespace SmartSolar.Device.Core.Common
{
	public class FakeOutputConnection : PropertyChangedBase, IOutputConnection
	{
		private bool? _state;

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