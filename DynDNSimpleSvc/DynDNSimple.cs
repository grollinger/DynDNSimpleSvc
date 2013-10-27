using System;
using System.Diagnostics;
using System.Net.Http;
using System.ServiceProcess;
using System.Threading;

namespace DynDNSimpleSvc {
    public partial class DynDNSimple : ServiceBase {
        public const string EventSource = "DynDNSSimpleSvc";

        DNSimpleClient _DNSimple;
        JSONIPClient _JSONIP;
        string _Domain, _DomainToken;
        int _RecordID;
        Timer _Timer;

        public DynDNSimple() {
            InitializeComponent();
        }

        protected override void OnStart(string[] args) {
            OnStart();
        }

        public void OnStart() {
            var interval = Int32.Parse(System.Configuration.ConfigurationManager.AppSettings["updateinterval_minutes"]);
            _Domain = System.Configuration.ConfigurationManager.AppSettings["domain"];
            _RecordID = Int32.Parse(System.Configuration.ConfigurationManager.AppSettings["recordid"]);
            _DomainToken = System.Configuration.ConfigurationManager.AppSettings["domain_token"];


            _DNSimple = new DNSimpleClient(_Domain, _DomainToken);
            _JSONIP = new JSONIPClient();

            

            _Timer = new Timer(OnTick);

            var intervalMs = interval * 1000 * 60;

            _Timer.Change(1, intervalMs);
        }

        void OnTick(object state) {
            update_dns();
        }

        async void update_dns() {
            try {
                var ipTask = _JSONIP.GetIP();
                var recordTask = _DNSimple.GetRecord(_RecordID);

                var ip = await ipTask;
                var record = await recordTask;

                if ((ip != null && record != null) &&
                    (ip != record.content))
                {
                    await _DNSimple.UpdateRecord(_RecordID, ip);
                }
            }
            catch (Exception ex) {


                EventLog.WriteEntry(EventSource,
                    string.Format("An Exception occurred:\n {0}", ex.ToString()),
                    EventLogEntryType.Warning, 1);
            }
        }

        protected override void OnStop() {
            OnStopped();
        }
        public void OnStopped() {
            _Timer.Dispose();
        }
    }
}
