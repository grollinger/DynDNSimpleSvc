using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;

namespace DynDNSimpleSvc
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main(string[] args)
        {
#if(!DEBUG)
            if (args.Length > 1)
            {
                Console.WriteLine("Checking Event Log...");
                if (!EventLog.SourceExists(DynDNSimple.EventSource))
                {
                    Console.WriteLine("Creating EventLog");
                    EventLog.CreateEventSource(DynDNSimple.EventSource, "Application");
                }
                else
                {
                    Console.WriteLine("...exists");
                }
            }
            else
            {
                ServiceBase[] ServicesToRun;
                ServicesToRun = new ServiceBase[] 
                { 
                    new DynDNSimple() 
                };
                ServiceBase.Run(ServicesToRun);
            }
#else
            var svc = new DynDNSimple();
            svc.OnStart();
            Console.WriteLine("Test");
            Console.ReadLine();
            svc.OnStopped();
#endif
        }
    }
}
