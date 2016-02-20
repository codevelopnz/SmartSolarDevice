using System;
using System.Collections.Generic;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.Devices.Gpio;
using Caliburn.Micro;
using Ninject;
using SmartSolar.Device.Core.Domain;
using SmartSolar.Device.Core.Services;
using SmartSolar.Device.Messages;
using SmartSolar.Device.ViewModels;

namespace SmartSolar.Device
{
	public sealed partial class App
	{
		private WinRTContainer _container;
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

			_container = new WinRTContainer();
			_container.RegisterWinRTServices();

//			_eventAggregator = _container.GetInstance<IEventAggregator>();
			// Singletons
//			_container
//				.Singleton<Settings>()
//				.Singleton<PumpController>();
			_kernel.Bind<Settings>().ToSelf().InSingletonScope();
			_kernel.Bind<PumpController>().ToSelf().InSingletonScope();
			var settings = _kernel.Get<Settings>();

			// Per-requests
//			_container
//				.PerRequest<ShellViewModel>()
//				.PerRequest<DeviceViewModel>()
//				.PerRequest<MainPageViewModel>()
//				.PerRequest<ReadoutViewModel>();


			// Workaround for Caliburn Micro being dumb about CI, TODO link to explanation about how it doesn't like to instantiate classes where the ancestor has a public controller. Switch to ninject after all?
//			PumpController singletonPumpController;
//			_container.Handler<PumpController>(c => {
//				if (singletonPumpController != null) return singletonPumpController; 
//				)
//			})
//				>()
//				.
			// Use real hardware if we have a GpioController - else use fakeys
			var shouldUseRealHardware = (GpioController.GetDefault() != null);

			if (shouldUseRealHardware)
			{
				// Get the pins we're going to use
				var gpioController = GpioController.GetDefault();
				var pumpGpioPin = gpioController.OpenPin(settings.PumpGpioPin);

				// Create an output connection on top of the pins
				_container.PerRequest<GpioOutputConnection>();
				var pumpConnection = _container.GetInstance<GpioPumpOutputConnection>();
				pumpConnection.Configure(pumpGpioPin);

				// Use these connections where requested
//				pump.Connection = pumpConnection;
				_kernel.Bind<IPumpOutputConnection>().ToConstant(pumpConnection);
			} else {
				// Register some fake connectors
//				_container.PerRequest<FakeOutputConnection>();

//				var fakePumpConnection = _kernel.Get<FakeOutputConnection>();
//				_container.RegisterInstance(typeof (IPumpOutputConnection), null, fakePumpConnection);
				_kernel.Bind<IPumpOutputConnection>().To<FakePumpOutputConnection>();
			}

			// Intercept the creation of any object, and configure it before it gets used.
			// https://caliburnmicro.codeplex.com/wikipage?title=The%20Simple%20IoC%20Container
//			_container.Activated += o =>
//			{
//				// Configure the PumpController with either a real or fake output connection
//				var pump = o as PumpController;
//				if (pump != null)
//				{
//					if (shouldUseRealHardware)
//					{
//						// Get a real GPIO pin
//						var gpioController = GpioController.GetDefault();
//						var pumpGpioPin = gpioController.OpenPin(settings.PumpGpioPin);
//						// Create an output connection on top of the pin
//						var pumpConnection = _container.GetInstance<GpioOutputConnection>();
//						pumpConnection.Configure(pumpGpioPin);
//						pump.Connection = pumpConnection;
//					}
//					else
//					{
//						// Use a fake output connection
//						pump.Connection = _container.GetInstance<FakeOutputConnection>();
//					}
//				}
//			};
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
