using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RestSharp;
using Newtonsoft.Json;

namespace DynDNSimpleSvc
{
    class JSONIPClient : RestSharp.RestClient
    {
        public async Task<string> GetIP()
        {
            var request = new RestRequest("http://jsonip.com", Method.GET);

            var response = await this.ExecuteTaskAsync(request);

            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                dynamic ipObject = JsonConvert.DeserializeObject(response.Content);
                return ipObject.ip;
            }
            else
            {
                return null;
            }
        }
    }
}
