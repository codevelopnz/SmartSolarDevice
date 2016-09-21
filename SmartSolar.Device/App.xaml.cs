using System;
using System.Collections.Generic;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.Devices.Gpio;
using Caliburn.Micro;
using Ninject;
using SmartSolar.Device.Core.Common;
using SmartSolar.Device.Core.Element;
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

			// Singletons
			_kernel.Bind<Settings>().ToSelf().InSingletonScope();
			_kernel.Bind<PumpController>().ToSelf().InSingletonScope();
			_kernel.Bind<ElementController>().ToSelf().InSingletonScope();
			_kernel.Bind<Hardware>().ToSelf().InSingletonScope();
			_kernel.Bind<HardwareInitializer>().ToSelf().InSingletonScope();
			var settings = _kernel.Get<Settings>();

			// Use real hardware if we have a GpioController - else use fakeys
			var shouldUseRealHardware = (GpioController.GetDefault() != null);

			if (shouldUseRealHardware)
			{
				// Configure the ADC for inputs
//				_kernel.Bind<IAnalogToDigitalConvertor>().To<Mcp3208>();
				_kernel.Bind<IAnalogToDigitalConvertor>().To<Mcp3008>().InSingletonScope();

				// Configure the GPIO for outputs
				var gpioController = GpioController.GetDefault();
				var pumpGpioPin = gpioController.OpenPin(settings.PumpGpioPin);
                var pumpLedGpioPin = gpioController.OpenPin(settings.PumpLedPin);
                var elementGpioPin = gpioController.OpenPin(settings.ElementGpioPin);
                var elementLedGpioPin = gpioController.OpenPin(settings.ElementLedPin);

                // Use real inputs/outputs where requested
                _kernel.Bind<IOutputConnection>().To<GpioOutputConnection>();
				_kernel.Bind<ITemperatureReader>().To<ThermistorTemperatureReader>();

				// Get and configure the hardware object with correct pins etc
				var hardware = _kernel.Get<Hardware>();
                // - GPIO Outputs
                (hardware.PumpOutputConnection as GpioOutputConnection)?.Configure(pumpGpioPin);
                (hardware.PumpLedOutputConnection as GpioOutputConnection)?.Configure(pumpLedGpioPin);
                (hardware.ElementOutputConnection as GpioOutputConnection)?.Configure(elementGpioPin);
                (hardware.ElementLedOutputConnection as GpioOutputConnection)?.Configure(elementLedGpioPin);
                // - ADC inputs
                ((ThermistorTemperatureReader) hardware.RoofTemperatureReader).PinNumber = settings.RoofThermistorAdcPin;
				((ThermistorTemperatureReader) hardware.TankTemperatureReader).PinNumber = settings.TankThermistorAdcPin;
				((ThermistorTemperatureReader) hardware.InletTemperatureReader).PinNumber = settings.InletThermistorAdcPin;
				// - Thermistor model parameters
				((ThermistorTemperatureReader) hardware.RoofTemperatureReader).ThermistorModelParameters = settings.RoofThermistorModelParameters;
				((ThermistorTemperatureReader) hardware.TankTemperatureReader).ThermistorModelParameters = settings.TankThermistorModelParameters;
				((ThermistorTemperatureReader) hardware.InletTemperatureReader).ThermistorModelParameters = settings.InletThermistorModelParameters;

			} else {
				// Use fake inputs / outputs where requested
				_kernel.Bind<IAnalogToDigitalConvertor>().To<FakeAnalogToDigitalConvertor>().InSingletonScope();
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
			return _kernel.Get(service);//, key);
		}

		protected override IEnumerable<object> GetAllInstances(Type service)
		{
			return _kernel.GetAll(service);
		}

		protected override void BuildUp(object instance)
		{
			_kernel.Inject(instance);
		}
	}
}
