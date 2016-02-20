using System;
using System.Collections.Generic;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.Devices.Gpio;
using Caliburn.Micro;
using Ninject;
using SmartSolar.Device.Core.Common;
using SmartSolar.Device.Core.Pump;
using SmartSolar.Device.Core.Sensor;
using SmartSolar.Device.Messages;
using SmartSolar.Device.ViewModels;

namespace SmartSolar.Device
{
	public sealed partial class App
	{
		private IEventAggregator _eventAggregator;
		private IKernel _kernel;

		public App()
		{
			InitializeComponent();
		}

		protected override void Configure()
		{
			_kernel = new Ninject.StandardKernel();

			_kernel.Bind<IEventAggregator>().To<EventAggregator>().InSingletonScope();
			_eventAggregator = _kernel.Get<IEventAggregator>();

//			_container = new WinRTContainer();
//			_container.RegisterWinRTServices();

			// Singletons
			_kernel.Bind<Settings>().ToSelf().InSingletonScope();
			_kernel.Bind<PumpController>().ToSelf().InSingletonScope();
			_kernel.Bind<Hardware>().ToSelf().InSingletonScope();
			var settings = _kernel.Get<Settings>();

			// Use real hardware if we have a GpioController - else use fakeys
			var shouldUseRealHardware = (GpioController.GetDefault() != null);

			if (shouldUseRealHardware)
			{
				// Configure the ADC for inputs
				_kernel.Bind<IAnalogToDigitalConvertor>().To<Mcp3208>();

				// Configure the GPIO for outputs
				var gpioController = GpioController.GetDefault();
				var pumpGpioPin = gpioController.OpenPin(settings.PumpGpioPin);
				var elementGpioPin = gpioController.OpenPin(settings.ElementGpioPin);

				// Use real inputs/outputs where requested
				_kernel.Bind<IOutputConnection>().To<GpioOutputConnection>();
				_kernel.Bind<ITemperatureReader>().To<ThermistorTemperatureReader>();

				// Get and configure the hardware object with correct pins etc
				_kernel.Bind<Hardware>().ToSelf().InSingletonScope();
				var hardware = _kernel.Get<Hardware>();
				(hardware.PumpOutputConnection as GpioOutputConnection)?.Configure(pumpGpioPin);
				(hardware.ElementOutputConnection as GpioOutputConnection)?.Configure(elementGpioPin);
				// TODO: configure the thermistor inputs
//				(hardware.InletTemperatureReader as ThermistorTemperatureReader)?.PinNumber = settings.;

			} else {
				// Use fake inputs / outputs where requested
				_kernel.Bind<IOutputConnection>().To<FakeOutputConnection>();
				_kernel.Bind<ITemperatureReader>().To<FakeTemperatureReader>();
				// Setup some initial values for fake readings
				var hardware = _kernel.Get<Hardware>();
				((FakeTemperatureReader) hardware.RoofTemperatureReader).FakeTemperatureDegC = 50;
				((FakeTemperatureReader) hardware.InletTemperatureReader).FakeTemperatureDegC = 40;
				((FakeTemperatureReader) hardware.TankTemperatureReader).FakeTemperatureDegC = 30;
				((FakeOutputConnection) hardware.PumpOutputConnection).State = false;
				((FakeOutputConnection) hardware.ElementOutputConnection).State = false;
			}

			// Kick off the poller
			var sensorPoller = _kernel.Get<SensorPoller>();
			sensorPoller.PollContinuously();
		}

		protected override void OnLaunched(LaunchActivatedEventArgs args)
		{
			// Note we're using DisplayRootViewFor (which is view model first)
			// this means we're not creating a root frame and just directly
			// inserting ShellView as the Window.Content

			DisplayRootViewFor<ShellViewModel>();

			// It's kinda of weird having to use the event aggregator to pass 
			// a value to ShellViewModel, could be an argument for allowing
			// parameters or launch arguments

			if (args.PreviousExecutionState == ApplicationExecutionState.Terminated)
			{
				_eventAggregator.PublishOnUIThread(new ResumeStateMessage());
			}
		}

		protected override void OnSuspending(object sender, SuspendingEventArgs e)
		{
			_eventAggregator.PublishOnUIThread(new SuspendStateMessage(e.SuspendingOperation));
		}

		protected override object GetInstance(Type service, string key)
		{
//			return _container.GetInstance(service, key);
			return _kernel.Get(service);//, key);
		}

		protected override IEnumerable<object> GetAllInstances(Type service)
		{
//			return _container.GetAllInstances(service);
			return _kernel.GetAll(service);
		}

		protected override void BuildUp(object instance)
		{
			_kernel.Inject(instance);
//			_container.BuildUp(instance);
		}
	}
}
