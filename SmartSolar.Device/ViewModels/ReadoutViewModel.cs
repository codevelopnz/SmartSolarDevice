using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Caliburn.Micro;
using SmartSolar.Device.Core.ApiClient;
using SmartSolar.Device.Core.Common;
using SmartSolar.Device.Core.Element;
using SmartSolar.Device.Core.Pump;
using SmartSolar.Device.Core.Sensor;

namespace SmartSolar.Device.ViewModels
{
	public class ReadoutViewModel : Screen
	{
		private readonly HardwareInitializer _hardwareInitializer;
	    private readonly ApiLinkManager _apiLinkManager;
	    public PumpController PumpController { get; set; }
		public ElementController ElementController { get; set; }
		public Hardware Hardware { get; set; }

		public string PumpStateText => 
			Hardware.PumpOutputConnection.State.HasValue
			? (Hardware.PumpOutputConnection.State.Value ? "On" : "Off") 
			: "?";
		public string ElementStateText => 
			Hardware.ElementOutputConnection.State.HasValue
			? (Hardware.ElementOutputConnection.State.Value ? "On" : "Off") 
			: "?";

		public string RoofDegC => Hardware.RoofTemperatureReader.LastTemperatureDegC.HasValue
			? Hardware.RoofTemperatureReader.LastTemperatureDegC.ToString()
			: "?";
		public string TankDegC => Hardware.TankTemperatureReader.LastTemperatureDegC.HasValue
			? Hardware.TankTemperatureReader.LastTemperatureDegC.ToString()
			: "?";
		public string InletDegC => Hardware.InletTemperatureReader.LastTemperatureDegC.HasValue
			? Hardware.InletTemperatureReader.LastTemperatureDegC.ToString()
			: "?";



		public ReadoutViewModel(
			PumpController pumpController,
			ElementController elementController,
			Hardware hardware,
			HardwareInitializer hardwareInitializer ,
            ApiLinkManager apiLinkManager
			)
		{
			_hardwareInitializer = hardwareInitializer;
		    _apiLinkManager = apiLinkManager;
		    PumpController = pumpController;
			ElementController = elementController;
			Hardware = hardware;

			// When the PumpController changes state, so should our text, so notify of that change.
			Hardware.PumpOutputConnection.PropertyChanged += (s, e) => NotifyOfPropertyChange(() => PumpStateText);
			Hardware.ElementOutputConnection.PropertyChanged += (s, e) => NotifyOfPropertyChange(() => ElementStateText);
			Hardware.RoofTemperatureReader.PropertyChanged += (s, e) => NotifyOfPropertyChange(() => RoofDegC);
			Hardware.InletTemperatureReader.PropertyChanged += (s, e) => NotifyOfPropertyChange(() => InletDegC);
			Hardware.TankTemperatureReader.PropertyChanged += (s, e) => NotifyOfPropertyChange(() => TankDegC);
		}
		protected override void OnActivate() {
			// Hardware initialization is async, and needs to be done somewhere where that won't hang
			// Here is a good place - see HardwareInitializer class for details
			_hardwareInitializer.Initialize();
            _apiLinkManager.ManageApiLink();
		}

		public void RoofDegCPlus()
		{
			var fake = (Hardware.RoofTemperatureReader as FakeTemperatureReader);
			if (fake != null)
			{
				fake.FakeTemperatureDegC += 3;
			}
		}
		public void RoofDegCMinus()
		{
			var fake = (Hardware.RoofTemperatureReader as FakeTemperatureReader);
			if (fake != null)
			{
				fake.FakeTemperatureDegC -= 3;
			}
		}
		public void InletDegCPlus()
		{
			var fake = (Hardware.InletTemperatureReader as FakeTemperatureReader);
			if (fake != null)
			{
				fake.FakeTemperatureDegC += 3;
			}
		}
		public void InletDegCMinus()
		{
			var fake = (Hardware.InletTemperatureReader as FakeTemperatureReader);
			if (fake != null)
			{
				fake.FakeTemperatureDegC -= 3;
			}
		}
		public void TankDegCPlus()
		{
			var fake = (Hardware.TankTemperatureReader as FakeTemperatureReader);
			if (fake != null)
			{
				fake.FakeTemperatureDegC += 3;
			}
		}
		public void TankDegCMinus()
		{
			var fake = (Hardware.TankTemperatureReader as FakeTemperatureReader);
			if (fake != null)
			{
				fake.FakeTemperatureDegC -= 3;
			}
		}
	}
}
