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

            string eventSourceName = "GPPSource";
            string logName = "GPPLog";

            if (args.Length > 0)
            {
                eventSourceName = args[0];
            }

            if (args.Length > 1)
            {
                logName = args[1];
            }

            eventLog1 = new EventLog();
            if (!EventLog.SourceExists("GPPSource"))
            {
                EventLog.CreateEventSource("GPPSource", "GPPLog");
            }
            eventLog1.Source = "GPPSource";
            eventLog1.Log = "GPPLog";
        }

        public MyNewService()
        {
        }

        protected override void OnStart(string[] args)
        {
            eventLog1.WriteEntry("In OnStart");
            StartVboxVM();
            eventLog1.WriteEntry("StartVboxVM executed");

            Timer timer = new Timer
            {
                Interval = 60000
            };
            timer.Elapsed += new ElapsedEventHandler(this.OnTimer);
            timer.Start();
        }

        protected override void OnStop()
        {
            eventLog1.WriteEntry("In OnStop");
            StopVboxVm();
            eventLog1.WriteEntry("StopVboxVM executed");
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

        public void StartVboxVM()
        {
            Process myProc = new Process();
            ExecuteCommand(@"cd C:\Program Files\Oracle\Virtualbox && vboxmanage startvm TestVm --type headless");
        }

        public void StopVboxVm()
        {
            Process myProc = new Process();
            ExecuteCommand(@"cd C:\Program Files\Oracle\Virtualbox && vboxmanage controlvm TestVm poweroff");
        }

        public void ExecuteCommand(string command)
        {
            int ExitCode;
            ProcessStartInfo ProcessInfo;
            Process Process;

            ProcessInfo = new ProcessStartInfo("cmd.exe", "/c " + command)
            {
                CreateNoWindow = true,
                UseShellExecute = false
            };

            Process = Process.Start(ProcessInfo);
            Process.WaitForExit();

            ExitCode = Process.ExitCode;
            Process.Close();
        }

    }
}
