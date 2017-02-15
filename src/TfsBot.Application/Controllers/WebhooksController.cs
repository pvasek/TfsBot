using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using Microsoft.ApplicationInsights;
using TfsBot.Common.Dtos;
using TfsBot.Common.Bot;
using TfsBot.Common.Db;

namespace TfsBot.Controllers
{
    [RoutePrefix("api/webhooks")]
    public class WebhooksController : ApiController
    {
        public WebhooksController(IRepository repository)
        {
            _repository = repository;
        }

        private readonly IRepository _repository;

        [HttpPost]
        [Route("pullrequest/{id}")]
        public async Task<HttpResponseMessage> PullRequest(string id, [FromBody] PullRequest req)
        {
            TrackEvent("pullrequest", id, req.EventType);
            var message = GetPullRequestMessage(req);
            await SendMessageIfDefined(id, message);
            return Request.CreateResponse(HttpStatusCode.OK);
        }

        [HttpPost]
        [Route("build/{id}")]
        public async Task<HttpResponseMessage> Build(string id, [FromBody] BuildRequest req)
        {
            TrackEvent("build", id, req.EventType);
            var message = GetBuildMessage(req);
            await SendMessageIfDefined(id, message);
            return Request.CreateResponse(HttpStatusCode.OK);
        }

        private static void TrackEvent(string webhookType, string id, string eventType)
        {
            var telemetry = new TelemetryClient();
            var trackParams = new Dictionary<string, string>
            {
                {"id", id},
                {"eventType", eventType},
                //{"content", contentString}
            };

            telemetry.TrackEvent($"webhooks.{webhookType}", trackParams);
        }

        private async Task SendMessageIfDefined(string id, IEnumerable<string> messages)
        {
            if (!messages.Any())
            {
                return;
            }

            var clients = _repository.GetServerClients(id);
            if (clients.Count == 0)
            {
                throw new ArgumentException($"There are no clients for id: {id}");
            }
            foreach (var client in clients)
            {
                await BotHelper.SendMessageToClient(client, string.Join(Environment.NewLine + Environment.NewLine, messages));
            }            
        }
               
        private static IEnumerable<string> GetPullRequestMessage(PullRequest req)
        {
            var prId = req.Resource.PullRequestId;
            yield return $"**PR{prId}** {req.Message.Markdown} ([link]({req.Resource.Repository.RemoteUrl}/pullrequest/{prId}?view=files))";
            if (req.EventType == "git.pullrequest.created")
            {
                yield return $"_**{req.Resource.Title}**_";
                yield return $"_{req.Resource.Description}_";
            }
        }

        private static IEnumerable<string> GetBuildMessage(BuildRequest req)
        {
            yield return $"**BUILD {req.Resource.BuildNumber}** {req.Message.Markdown} ([link]({req.Resource.Url}))";
        }
    }
}