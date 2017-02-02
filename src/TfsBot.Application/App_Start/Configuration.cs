using Microsoft.Azure;

namespace TfsBot
{
    public class Configuration
    {
        public string StorageConnectionString => CloudConfigurationManager.GetSetting("StorageConnectionString");
        public string ApplicationInsightsKey => CloudConfigurationManager.GetSetting("ApplicationInsightsKey");
    }
}