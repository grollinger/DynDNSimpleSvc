using System;
using System.Diagnostics;
using System.ServiceProcess;
using DNSimple;
using System.Net.Http;
using System.Threading;

namespace DynDNSimpleSvc
{
    public partial class DynDNSimple : ServiceBase
    {
        public const string EventSource = "DynDNSSimpleSvc";

        DNSimpleRestClient _DNSimple;
        string _Domain, _RecordName;
        int _RecordID;
        Timer _Timer;

        public DynDNSimple()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            OnStart();
        }

        public void OnStart()
        {
            var interval = Int32.Parse(System.Configuration.ConfigurationManager.AppSettings["updateinterval_minutes"]);
            var user = System.Configuration.ConfigurationManager.AppSettings["username"];
            var pass = System.Configuration.ConfigurationManager.AppSettings["password"];
            _Domain = System.Configuration.ConfigurationManager.AppSettings["domain"];
            _RecordID = Int32.Parse(System.Configuration.ConfigurationManager.AppSettings["recordid"]);
            _RecordName = System.Configuration.ConfigurationManager.AppSettings["recordname"];

            

            _DNSimple = new DNSimpleRestClient(user, pass);

            _Timer = new Timer(OnTick);

            var intervalMs = interval * 1000 * 60;

            _Timer.Change(1, intervalMs);
        }

        void OnTick(object state)
        {
            Console.WriteLine("Tick");
            update_dns();            
        }

        async void update_dns()
        {
            try
            {
                var ip_json = await new HttpClient().GetStringAsync("http://jsonip.com");

                var rdr = new JsonFx.Json.JsonReader();

                dynamic json = rdr.Read(ip_json);
                string ip = (string)json.ip;

                var response = _DNSimple.UpdateRecord(_Domain, _RecordID, _RecordName, ip);
            }
            catch (Exception ex)
            {
                

                EventLog.WriteEntry(EventSource,
                    string.Format("An Exception occurred:\n {0}", ex.ToString()), 
                    EventLogEntryType.Warning, 1);
            }
        }

        protected override void OnStop()
        {
            OnStopped();
        }
        public void OnStopped()
        {
            _Timer.Dispose();
        }
    }
}
