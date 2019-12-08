using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace MyNewService
{
    public partial class MyNewService : ServiceBase
    {
        private int eventId = 1;

        public MyNewService(string[] args)
        {
            InitializeComponent();

            string eventSourceName = "MySource";
            string logName = "MyNewLog";

            if (args.Length > 0)
            {
                eventSourceName = args[0];
            }

            if (args.Length > 1)
            {
                logName = args[1];
            }

            eventLog1 = new EventLog();
            if (!EventLog.SourceExists("MySource"))
            {
                EventLog.CreateEventSource("MySource", "MyNewLog");
            }
            eventLog1.Source = "MySource";
            eventLog1.Log = "MyNewLog";
        }

        protected override void OnStart(string[] args)
        {
            eventLog1.WriteEntry("This is my service start");
            eventLog1.WriteEntry("In OnStart");
            StartVboxVM();

            Timer timer = new Timer
            {
                Interval = 60000
            };
            timer.Elapsed += new ElapsedEventHandler(this.OnTimer);
            timer.Start();
        }

        protected override void OnStop()
        {
            eventLog1.WriteEntry("In OnStop.");
            StopVboxVm();
        }

        private void OnTimer(object sender, ElapsedEventArgs e)
        {
            // TODO: Insert monitoring activities here.
            eventLog1.WriteEntry("Monitoring the System", EventLogEntryType.Information, eventId++);
        }

        protected override void OnContinue()
        {
            eventLog1.WriteEntry("In OnContinue.");
        }

        private void StartVboxVM()
        {
            //process.StartInfo.RedirectStandardOutput = true;
            //System.Diagnostics.Process.Start("CMD.exe", "/K ipconfig");
            //Process.Start("cmd.exe", @"cd C:\Program Files\Oracle\Virtualbox && vboxmanage startvm TestVm --type headless");
            //try
            //{
            //    System.Diagnostics.Process.Start("CMD.exe", "/K ipconfig");
            //    //Process process = new Process();
            //    //process.StartInfo.FileName = @"cmd.exe";
            //    ////process.StartInfo.WorkingDirectory = @"C:\Program Files\Oracle\Virtualbox\";
            //    //process.StartInfo.UseShellExecute = true;
            //    //process.StartInfo.Arguments = @"cd C:\Program Files\Oracle\Virtualbox && vboxmanage startvm TestVm headless";
            //    //process.Start();
            //} catch (Exception e)
            //{
            //    Console.WriteLine(e);
            //}
            try
            {
                using (Process myProcess = new Process())
                {
                    myProcess.StartInfo.UseShellExecute = false;
                    // You can start any process, HelloWorld is a do-nothing example.
                    myProcess.StartInfo.FileName = @"cmd.exe";
                    myProcess.StartInfo.Arguments = @"cd C:\Program Files\Oracle\Virtualbox && vboxmanage startvm TestVm headless";
                    myProcess.StartInfo.CreateNoWindow = true;
                    myProcess.Start();
                    myProcess.Kill();
                    // This code assumes the process you are starting will terminate itself. 
                    // Given that is is started without a window so you cannot terminate it 
                    // on the desktop, it must terminate itself or you can do it programmatically
                    // from this application using the Kill method.
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

        }

        private void StopVboxVm()
        {
            //Process process = new Process();
            //process.StartInfo.FileName = "cmd.exe";
            //process.StartInfo.WorkingDirectory = @"C:\Program Files\Oracle\Virtualbox\";
            //process.StartInfo.Arguments = "vboxmanage controlvm TestVm poweroff";
            //process.Start();
            try
            {
                using (Process myProcess = new Process())
                {
                    myProcess.StartInfo.UseShellExecute = false;
                    // You can start any process, HelloWorld is a do-nothing example.
                    myProcess.StartInfo.FileName = @"cmd.exe";
                    myProcess.StartInfo.Arguments = @"cd C:\Program Files\Oracle\Virtualbox && vboxmanage startvm TestVm headless";
                    myProcess.StartInfo.CreateNoWindow = true;
                    myProcess.Start();
                    myProcess.Kill();
                    // This code assumes the process you are starting will terminate itself. 
                    // Given that is is started without a window so you cannot terminate it 
                    // on the desktop, it must terminate itself or you can do it programmatically
                    // from this application using the Kill method.
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

    }
}
