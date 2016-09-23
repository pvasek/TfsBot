using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using Microsoft.ApplicationInsights;
using Microsoft.Bot.Connector;
using Newtonsoft.Json.Linq;
using TfsBot.Common.Db;
using TfsBot.Common.Entities;

namespace TfsBot.Controllers
{
    public class TfsCallbackController : ApiController
    {
        [HttpPost]
        public async Task<HttpResponseMessage> Post(string id)
        {
            var telemetry = new TelemetryClient();

            var contentString = await Request.Content.ReadAsStringAsync();
            var content = JObject.Parse(contentString);
            var eventType = content["eventType"].ToString();

            telemetry.TrackEvent("TfsCallback.Post", new Dictionary<string, string>
                    {
                        {"id", id},
                        {"eventType", eventType},
                        {"content", contentString}
                    });

            var message = GetMessage(content);
            if (message == null)
            {
                return Request.CreateResponse(HttpStatusCode.OK);
            }

            var repository = new Repository();
            var clients = repository.GetServerClients(id);
            foreach (var client in clients)
            {
                await SendMessage(client, message);
            }

            return Request.CreateResponse(HttpStatusCode.OK);
        }

        [HttpGet]
        public async Task<HttpResponseMessage> Test(string id)
        {
            var telemetry = new TelemetryClient();
            telemetry.TrackEvent("TfsCallback.Test", new Dictionary<string, string>
                    {
                        {"id", id}
                    });

            var repository = new Repository();
            var clients = repository.GetServerClients(id);
            foreach (var client in clients)
            {
                try
                {
                    await SendMessage(client, GetMessage(null));
                }
                catch (Exception e)
                {
                    telemetry.TrackException(e);
                    telemetry.TrackEvent("SendMessageFailed", new Dictionary<string, string>
                    {
                        {"userId", client.UserId},
                        {"userName", client.UserName }
                    });
                    //await repository.RemoveServerClientAsync(client);
                }  
            }
            return Request.CreateResponse(HttpStatusCode.OK);
        }

        private string GetMessage(JObject content)
        {
            var file = System.Web.Hosting.HostingEnvironment.MapPath("~/tfs_pr_update_example2.json");
            content = content ?? JObject.Parse(File.ReadAllText(file));
            var message = content["message"];
            var eventType = content["eventType"].ToString();
            var resource = content["resource"];
            var prId = resource["pullRequestId"].ToString();
            var prTitle = resource["title"].ToString();
            var prUrl = resource["url"].ToString();
            var createdBy = resource["createdBy"];
            var createdByDisplayName = createdBy["displayName"].ToString();
            var createdByImageUrl = createdBy["imageUrl"].ToString();
            var repository = resource["repository"];
            var remoteUrl = repository["remoteUrl"].ToString();

            if (eventType == "git.pullrequest.created")
            {
                var msg = new StringBuilder($"**PR{prId}** [Please review PR{prId}]({remoteUrl}/pullrequest/{prId}?view=files)");
                msg.AppendLine();
                msg.AppendLine();
                msg.AppendLine(prTitle);
                msg.AppendLine();
                msg.AppendLine($"_{createdByDisplayName}_");
                return msg.ToString();
            }
            if (eventType == "git.pullrequest.updated")
            {
                return $"**PR{prId}** - {message["markdown"]}";
            }

            return null;
            //var cardImages = new List<CardImage>();
            //cardImages.Add(new CardImage(url: createdByImageUrl));
            //var cardButtons = new List<CardAction>();
            //var plButton = new CardAction
            //{
            //    Value = prUrl,
            //    Type = "openUrl",
            //    Title = "Open"
            //};
            //cardButtons.Add(plButton);
            //var plCard = new ThumbnailCard()
            //{
            //    Title = $"PR{prId}",
            //    Subtitle = prTitle,
            //    Images = cardImages,
            //    Buttons = cardButtons
            //};

            //message.Attachments = new List<Attachment>
            //{
            //    plCard.ToAttachment()
            //};
        }

        private static async Task SendMessage(ServerClient client, string messageText)
        {
            await SendMessage(client, msg => msg.Text = messageText);
        }

        private static async Task SendMessage(ServerClient client, Action<IMessageActivity> setMessage)
        {
            var connector = new ConnectorClient(new Uri(client.BotServiceUrl), new MicrosoftAppCredentials());
            var userAccount = new ChannelAccount(name: client.UserName, id: client.UserId);
            var botAccount = new ChannelAccount(name: client.BotName, id: client.BotId);
            var conversationId = await connector.Conversations.CreateDirectConversationAsync(botAccount, userAccount);

            ConversationParameters parameters = new ConversationParameters
            {
                
            };
            //connector.Conversations.CreateConversationAsync(parameters)
            var message = Activity.CreateMessageActivity();
            message.From = botAccount;
            message.Recipient = userAccount;
            message.Conversation = new ConversationAccount(id: conversationId.Id);
            message.Locale = "en-Us";
            setMessage(message);
            var response = await connector.Conversations.SendToConversationAsync((Activity) message);
        }
    }
}