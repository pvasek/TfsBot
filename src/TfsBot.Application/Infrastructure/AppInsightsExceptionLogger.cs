using Microsoft.ApplicationInsights;
using System.Collections.Generic;
using System.Web.Http.ExceptionHandling;

namespace TfsBot.Infrastructure
{
    public class AppInsightsExceptionLogger: ExceptionLogger
    {
        public override void Log(ExceptionLoggerContext context)
        {
            base.Log(context);
            var telemetry = new TelemetryClient();
            var parameters = new Dictionary<string, string>
            {
                { "RequestUri", context.ExceptionContext.Request.RequestUri.ToString() }
            };
            telemetry.TrackException(context.Exception, parameters);
        }    
    }
}