using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using Microsoft.ApplicationInsights;
using TfsBot.Common.Dtos;
using TfsBot.Common.Bot;

namespace TfsBot.Controllers
{
    public class TfsCallbackController : ApiController
    {
        [HttpPost]
        [Route("pullrequest")]
        public async Task<HttpResponseMessage> PullRequest(string id, [FromBody] PullRequest req)
        {
            TrackEvent(id, req.EventType);
            var message = GetPullRequestMessage(req);
            await SendMessageIfDefined(id, message);
            return Request.CreateResponse(HttpStatusCode.OK);
        }

        [HttpPost]
        [Route("build")]
        public async Task<HttpResponseMessage> Build(string id, [FromBody] BuildRequest req)
        {
            TrackEvent(id, req.EventType);
            var message = GetBuildMessage(req);
            await SendMessageIfDefined(id, message);
            return Request.CreateResponse(HttpStatusCode.OK);
        }

        private void TrackEvent(string id, string eventType)
        {
            var telemetry = new TelemetryClient();
            var trackParams = new Dictionary<string, string>
            {
                {"id", id},
                {"eventType", eventType},
                //{"content", contentString}
            };

            telemetry.TrackEvent("TfsCallback.Post", trackParams);
        }

        private async Task SendMessageIfDefined(string id, string message)
        { 
            if (message == null)
            {
                return;
            }

            var repository = new Common.Db.Repository();
            var clients = repository.GetServerClients(id);
            foreach (var client in clients)
            {
                await BotHelper.SendMessageToClient(client, message);
            }            
        }
               
        private string GetPullRequestMessage(PullRequest req)
        {
            var prId = req.Resource.PullRequestId;
            if (req.EventType == "git.pullrequest.created")
            {
                var msg = new StringBuilder($"**PR{prId}** [Please review PR{prId}]({req.Resource.Repository.RemoteUrl}/pullrequest/{prId}?view=files)");
                msg.AppendLine();
                msg.AppendLine();
                msg.AppendLine(req.Resource.Title);
                msg.AppendLine();
                msg.AppendLine($"_{req.Resource.CreatedBy.DisplayName}_");
                return msg.ToString();
            }

            if (req.EventType == "git.pullrequest.updated")
            {
                return $"**PR{prId}** - {req.Message.Markdown}";
            }

            return null;
        }

        private string GetBuildMessage(BuildRequest req)
        {
            return $"**Build [{req.Resource.BuildNumber}]({req.Resource.Url})** - {req.Resource.Status} ({req.Resource.LastChangedBy.DisplayName})";
        }
    }
}