using System.Web.Http;
using Microsoft.ApplicationInsights.Extensibility;
using TfsBot.App_Start;

namespace TfsBot
{
    public class WebApiApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {            
            GlobalConfiguration.Configure(WebApiConfig.Register);
            var container = SimpleInjectorInitializer.Initialize();
            MvcConfig.Configure();                 

            var config = container.GetInstance<Configuration>();

            // set application insights key
            var appKey = config.ApplicationInsightsKey;
            if (string.IsNullOrWhiteSpace(appKey))
            {
                TelemetryConfiguration.Active.DisableTelemetry = true;
            }
            else
            {
                TelemetryConfiguration.Active.InstrumentationKey = appKey;
            }            
        }
    }
}
