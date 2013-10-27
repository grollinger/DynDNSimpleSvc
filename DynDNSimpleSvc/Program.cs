using System;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;

namespace DynDNSimpleSvc {
    static class Program {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main(string[] args) {

            if (args.Contains("install")) {
                Console.WriteLine("Checking Event Log...");
                if (!EventLog.SourceExists(DynDNSimple.EventSource)) {
                    Console.WriteLine("Creating EventLog");
                    EventLog.CreateEventSource(DynDNSimple.EventSource, "Application");
                }
                else {
                    Console.WriteLine("...exists");
                }
            }
            else if (args.Contains("console")) {
                Console.WriteLine("DynDNSimpleSvc...");

                var svc = new DynDNSimple();

                svc.OnStart();

                Console.ReadLine();

                svc.OnStopped();
            }
            else {
                ServiceBase[] ServicesToRun;
                ServicesToRun = new ServiceBase[] 
                { 
                    new DynDNSimple() 
                };
                ServiceBase.Run(ServicesToRun);
            }
        }
    }
}
