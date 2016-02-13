using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Caliburn.Micro;
using SmartSolar.Device.Core.Domain;

namespace SmartSolar.Device.ViewModels
{
	public class ReadoutViewModel : Screen
	{
		public Pump Pump { get; set; }

		public string PumpStateText => 
			Pump.IsPumping.HasValue 
			? (Pump.IsPumping.Value ? "On" : "Off") 
			: "?";

		public ReadoutViewModel(Pump pump)
		{
			Pump = pump;

			// When the pump changes state, so should our text, so notify of that change.
			Pump.PropertyChanged +=
				(s, e) => { if (e.PropertyName == nameof(Pump.IsPumping)) NotifyOfPropertyChange(() => PumpStateText); };
		}
	}
}
