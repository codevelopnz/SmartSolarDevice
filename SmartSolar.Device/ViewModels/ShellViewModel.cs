using Windows.UI.Xaml.Controls;
using Caliburn.Micro;
using SmartSolar.Device.Messages;
using SmartSolar.Device.Views;

namespace SmartSolar.Device.ViewModels
{
    public class ShellViewModel : Screen, IHandle<ResumeStateMessage>, IHandle<SuspendStateMessage>
    {
        private readonly WinRTContainer _container;
        private readonly IEventAggregator _eventAggregator;
        private INavigationService _navigationService;
        private bool _resume;

		public MainPageView MainPage { get; set; }

        public ShellViewModel(WinRTContainer container, IEventAggregator eventAggregator)
        {
            _container = container;
            _eventAggregator = eventAggregator;
			MainPage = new MainPageView();
        }

        protected override void OnActivate()
        {
            _eventAggregator.Subscribe(this);
        }

        protected override void OnDeactivate(bool close)
        {
            _eventAggregator.Unsubscribe(this);
        }

        public void SetupNavigationService(Frame frame)
        {
            _navigationService = _container.RegisterNavigationService(frame);

	        if (_resume)
	        {
		        _navigationService.ResumeState();
	        }
	        else
	        {
				_navigationService.For<MainPageViewModel>().Navigate();
		        
	        }
					;
        }

        public void ShowDevices()
        {
//            _navigationService.For<DeviceViewModel>().Navigate();
            _navigationService.For<MainPageViewModel>().Navigate();
        }

        public void Handle(SuspendStateMessage message)
        {
            _navigationService.SuspendState();
        }

        public void Handle(ResumeStateMessage message)
        {
            _resume = true;
        }
    }
}
