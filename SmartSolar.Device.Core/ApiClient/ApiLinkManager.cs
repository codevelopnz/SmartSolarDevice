using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Caliburn.Micro;
using SmartSolar.Device.Core.Messages;

namespace SmartSolar.Device.Core.ApiClient
{
    // Single responsibility: to manage the link between the device and the API.
    // This includes polling to establishing the initial connection, and then passing data
    // between the device and the API.
    public class ApiLinkManager
    {
		private readonly IEventAggregator _eventAggregator;
        private readonly ApiClient _apiClient;

        public ApiLinkManager(IEventAggregator eventAggregator, ApiClient apiClient)
        {
            _eventAggregator = eventAggregator;
            _apiClient = apiClient;
        }

		public void ManageApiLink()
		{
			new TaskFactory().StartNew(async () =>
			{
				while(true) {
					TryConnect();
//					_eventAggregator.PublishOnUIThread(new SensorsWereRead());
					await Task.Delay(TimeSpan.FromMinutes(1));
				}
			});
		}

        private void TryConnect()
        {
            _apiClient.TryConnect();
        }
    }
}
