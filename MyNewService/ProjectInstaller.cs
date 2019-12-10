using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Configuration.Install;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.ServiceProcess;
using System.Threading.Tasks;

namespace MyNewService
{
    [RunInstaller(true)]
    public partial class ProjectInstaller : System.Configuration.Install.Installer
    {

        public ProjectInstaller(string[] args)
        {
            InitializeComponent();
        }

        protected override void OnBeforeInstall(IDictionary savedState)
        {
            string parameter = "MySource1\" \"MyLogFile1";
            Context.Parameters["assemblypath"] = "\"" + Context.Parameters["assemblypath"] + "\" \"" + parameter + "\"";
            base.OnBeforeInstall(savedState);

            //string password = GetContextParameter("upassword").Trim();
            //if (password != "")
            //    serviceProcessInstaller.Password = password;

            //string gppName = GetContextParameter("gppName").Trim();
            //if (!string.IsNullOrEmpty(gppName))
            //    WriteToSettings("gppName", gppName);

            string appDir = GetContextParameter("appdir").Trim();
            string ovaName = FindOva(appDir);
            eventLog1.WriteEntry("passed in appdir: " + appDir);
            eventLog1.WriteEntry("passed in ovaName: " + ovaName);
            ReadAllSettings();
            if (!string.IsNullOrEmpty(ovaName))
            {
                AddUpdateAppSettings("gppName", "updateThroughBefore");
                //WriteToSettings("gppName", ovaName);
            }
            ReadAllSettings();


        }

        public string GetContextParameter(string key)
        {
            string sValue = "";
            try
            {
                sValue = this.Context.Parameters[key].ToString();
            }
            catch
            {
                sValue = "";
            }
            return sValue;
        }

        public void WriteToSettings(string key, string value)
        {
            try
            {
                var configFile = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
                var settings = configFile.AppSettings.Settings;
                if (settings["gppName"] == null)
                {
                    settings.Add(key, value);
                }
                configFile.Save(ConfigurationSaveMode.Modified);
                ConfigurationManager.RefreshSection(configFile.AppSettings.SectionInformation.Name);
            }
            catch (ConfigurationErrorsException)
            {
               
            }
        }

        public string FindOva(string appDir)
        {
            string gppDir = Path.Combine(appDir, "testfolder");
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
