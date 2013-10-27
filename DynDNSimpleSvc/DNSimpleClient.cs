using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RestSharp;
using System.Dynamic;
using Newtonsoft.Json;

namespace DynDNSimpleSvc
{
    class DNSimpleClient : RestSharp.RestClient
    {
        private string _Domain;

        public DNSimpleClient(string domain, string domain_token)
            : base("https://dnsimple.com/")
        {
            this.AddDefaultHeader("X-DNSimple-Domain-Token", domain_token);
            this.AddDefaultHeader("Accept", "application/json");            

            this._Domain = domain;
        }

        private RestRequest GetRecordRequest(int RecordID)
        {
            var request = new RestRequest("domains/{domain}/records/{id}");
            request.RequestFormat = DataFormat.Json;
            request.AddUrlSegment("domain", _Domain);
            request.AddUrlSegment("id", RecordID.ToString()); // replaces matching token in request.Resource

            return request;
        }

        public async Task<dynamic> GetRecord(int RecordID)
        {
            var request = GetRecordRequest(RecordID);


            var response = await this.ExecuteGetTaskAsync(request);

            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                return JsonConvert.DeserializeObject(response.Content);
            }
            else
            {
                return null;
            }
        }

        public async Task<dynamic> UpdateRecord(int RecordID, string IP)
        {
            var request = GetRecordRequest(RecordID);
            request.Method = Method.PUT;

            dynamic body = new ExpandoObject();
            body.record = new ExpandoObject();
            body.record.content = IP;

            request.AddBody(body);

            var response = await this.ExecuteTaskAsync(request);

            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                return JsonConvert.DeserializeObject(response.Content);
            }
            else
            {
                return null;
            }
        }
    }
}
