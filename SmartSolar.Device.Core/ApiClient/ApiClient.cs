using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNet.SignalR.Client;
using SmartSolar.Device.Core.Plumbing;

namespace SmartSolar.Device.Core.ApiClient
{
    public class ApiClient
    {
		private int _counter;
		private HubConnection _hubConnection;
		private IHubProxy _hubProxy;
		private bool _connectionStable;
		private Guid _deviceId;


        public ApiClient()
        {
//            _httpClient = new HttpClient();
//            _httpClient.BaseAddress = new Uri("http://localhost/CodevelopService");
        }
        public void TryConnect()
        {
//			_hubConnection = new HubConnection("http://172.16.0.17/CodevelopService");
			_hubConnection = new HubConnection("http://localhost/CodevelopService");
			//_hubConnection = new HubConnection(@"http://smartsolar.azurewebsites.net");
			var writer = new DebugTextWriter();
			_hubConnection.TraceLevel = TraceLevels.All;
			_hubConnection.TraceWriter = writer;
			_hubConnection.Error += _hubConnection_Error;
			_hubConnection.StateChanged += _hubConnection_StateChanged;


			// set up backchannel
			_hubProxy = _hubConnection.CreateHubProxy("DeviceFeedHub");

			_hubProxy.On<string>("hello", message =>
				LogMessage(message)
			);


			LogMessage("Starting");
			_hubConnection.Start().RunSynchronously();
		}

		void _hubConnection_StateChanged(StateChange obj)
		{

			LogMessage(_hubConnection.State.ToString());

			if (_hubConnection.State == ConnectionState.Connected)
			{
				_connectionStable = true;
			}
		}

	    private void _hubConnection_Error(Exception err)
	    {
		    LogMessage(err.Message);
	    }





	    private async void LogMessage(string s)
		{
//			await _dispatcher.RunAsync(CoreDispatcherPriority.High, () => serverFeed.Items.Add(s)); //add to listbox
		}
	}

}
