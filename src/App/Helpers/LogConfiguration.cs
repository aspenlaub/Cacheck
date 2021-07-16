using System;
using System.Diagnostics;
using Aspenlaub.Net.GitHub.CSharp.TashClient.Interfaces;

namespace Aspenlaub.Net.GitHub.CSharp.Cacheck.App.Helpers {
    public class LogConfiguration : ILogConfiguration {
        public string LogSubFolder => @"AspenlaubLogs\Cacheck";
        public string LogId => $"{DateTime.Today:yyyy-MM-dd}-{Process.GetCurrentProcess().Id}";
        public bool DetailedLogging => false;
    }
}
