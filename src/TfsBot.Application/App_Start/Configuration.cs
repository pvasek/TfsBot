using System.Configuration;
using Microsoft.Azure;

namespace TfsBot
{
    public class Configuration
    {
        public string StorageConnectionString => Get("StorageConnectionString");
        public string ApplicationInsightsKey => Get("ApplicationInsightsKey");
        public string Url => Get("Url");
        public string ServerIdPrefix => Get("ServerIdPrefix");

        public string Get(string key)
        {
            return CloudConfigurationManager.GetSetting(key) ?? ConfigurationManager.AppSettings[key];
        }

        public static Configuration Default { get; } = new Configuration();
    }
}