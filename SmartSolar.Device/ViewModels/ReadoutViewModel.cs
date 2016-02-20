using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Caliburn.Micro;
using SmartSolar.Device.Core.Domain;
using SmartSolar.Device.Core.Services;

namespace SmartSolar.Device.ViewModels
{
	public class ReadoutViewModel : Screen
	{
		public PumpController PumpController { get; set; }

		public string PumpStateText => 
			PumpController.IsPumping.HasValue 
			? (PumpController.IsPumping.Value ? "On" : "Off") 
			: "?";

		public ReadoutViewModel(PumpController pumpController)
		{
			PumpController = pumpController;

			// When the PumpController changes state, so should our text, so notify of that change.
			PumpController.PropertyChanged +=
				(s, e) => { if (e.PropertyName == nameof(PumpController.IsPumping)) NotifyOfPropertyChange(() => PumpStateText); };
		}
	}
}
