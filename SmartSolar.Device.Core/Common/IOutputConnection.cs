using System.ComponentModel;

namespace SmartSolar.Device.Core.Common
{
	/// <summary>
	///     Single responsibility: represent the pump to the rest of the application, including its current
	///     state, and the ability to turn it off or on.
	///     Note that because this inherits from PropertyChangedBase, users can register to be told of state changes.
	/// </summary>
	public interface IOutputConnection : INotifyPropertyChanged
	{
		bool? State { get; set; }
	}
}