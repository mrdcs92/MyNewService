using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.IO;
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
            ReadSetting("gppName");
            ReadAllSettings();
            AddUpdateAppSettings("gppName", "ohyea");
            ReadAllSettings();
            //eventLog1.WriteEntry("Attempting to find ova file");
            //eventLog1.WriteEntry(FindOva());
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

        public string FindOva()
        {
            string currentDir = Directory.GetCurrentDirectory();
            eventLog1.WriteEntry("current directory: " + currentDir);
            var parentDir = Directory.GetParent(currentDir).ToString();
            string gppDir = Path.Combine(parentDir, "testfolder");
            if (Directory.Exists(gppDir))
            {
                List<string> fileNames = Directory
                    .GetFiles(gppDir, "*.ova")
                    .Select(fileName => Path.GetFileNameWithoutExtension(fileName))
                    .ToList();
                return fileNames[0];
            }
            return "";
        }

        public void AddUpdateAppSettings(string key, string value)
        {
            try
            {
                var configFile = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
                var settings = configFile.AppSettings.Settings;
                if (settings[key] == null)
                {
                    settings.Add(key, value);
                }
                else
                {
                    settings[key].Value = value;
                }
                configFile.Save(ConfigurationSaveMode.Modified);
                ConfigurationManager.RefreshSection(configFile.AppSettings.SectionInformation.Name);
            }
            catch (ConfigurationErrorsException)
            {
                eventLog1.WriteEntry("Error writing app settings");
            }
        }

        public string ReadSetting(string key)
        {
            string result = "";
            try
            {
                var appSettings = ConfigurationManager.AppSettings;
                result = appSettings["gppName"] ?? "Not Found";
                eventLog1.WriteEntry("This is the result of reading the appSettings: " + result);
            }
            catch (ConfigurationErrorsException)
            {
                Console.WriteLine("Error reading app settings");
            }
            return result;
        }

        public void ReadAllSettings()
        {
            try
            {
                var appSettings = ConfigurationManager.AppSettings;

                if (appSettings.Count == 0)
                {
                    eventLog1.WriteEntry("AppSettings is empty.");
                }
                else
                {
                    foreach (var key in appSettings.AllKeys)
                    {
                        eventLog1.WriteEntry("Key: " + key + " Value: " + appSettings[key]);
                    }
                }
            }
            catch (ConfigurationErrorsException)
            {
                eventLog1.WriteEntry("Error reading app settings");
            }
        }

    }
}
