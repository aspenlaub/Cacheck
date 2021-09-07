using System;
using Aspenlaub.Net.GitHub.CSharp.TashClient.Interfaces;

namespace Aspenlaub.Net.GitHub.CSharp.Cacheck.Helpers {
    public class LogConfiguration : ILogConfiguration {
        public string LogSubFolder => @"AspenlaubLogs\Cacheck";
        public string LogId => $"{DateTime.Today:yyyy-MM-dd}-{Environment.ProcessId}";
        public bool DetailedLogging => false;
    }
}
