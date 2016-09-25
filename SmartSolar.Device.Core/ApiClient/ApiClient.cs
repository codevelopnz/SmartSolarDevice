using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace SmartSolar.Device.Core.ApiClient
{
    public class ApiClient
    {
        private readonly HttpClient _httpClient;

        public ApiClient()
        {
            _httpClient = new HttpClient();
            _httpClient.BaseAddress = new Uri("http://localhost/CodevelopService");
        }
        public void Authenticate(string username, string password)
        {
            var content = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("grant_type", "password"),
                new KeyValuePair<string, string>("username", "testUser"),
                new KeyValuePair<string, string>("password", "Password!"),
            });
            var result = _httpClient.PostAsync("/token", content).Result;
            string resultContent = result.Content.ReadAsStringAsync().Result;
            //                Console.WriteLine(resultContent);
        }
    }

}
