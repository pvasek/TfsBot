using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using Microsoft.ApplicationInsights;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;
using TfsBot.Common.Db;
using TfsBot.Common.Entities;

namespace TfsBot.Controllers
{
    [BotAuthentication]
    [Route("api/messages")]
    public class MessagesController : ApiController
    {
        public MessagesController(IRepository repository)
        {
            _repository = repository;
        }

        private readonly IRepository _repository;
        private const string SetServerCmd = "setserver:";
        private const string GetServerCmd = "getserver";
        private const string HelpCmd = "help";
        private const string GetUsersCmd = "getusers";

        /// <summary>
        /// POST: api/Messages
        /// Receive a message from a user and reply to it
        /// </summary>
        [HttpPost]
        public async Task<HttpResponseMessage> Post([FromBody]Activity activity)
        {
            var response = Request.CreateResponse(HttpStatusCode.OK);
            if (activity.Type == ActivityTypes.Message)
            {
                var messageText = activity.RemoveRecipientMention().Trim();
                TrackMessage(messageText);
                if (messageText.StartsWith(SetServerCmd))
                {
                    var serverParams = ServerParams.Parse(messageText.Substring(SetServerCmd.Length));
                    await SetServerIdAsync(activity, serverParams);
                    await SendReplyAsync(activity, $"Your server id was set to '{serverParams}'");
                    return response;
                }

                if (messageText == GetServerCmd)
                {
                    var serverId = await GetServerIdAsync(activity);
                    await SendReplyAsync(activity, $"Your server id is: '{serverId}'");
                    return response;
                }

                if (messageText == HelpCmd || messageText == "Settings")
                {
                    await SendReplyAsync(activity, $"You can setup your server id by writing _setserver:[server id]_ or getting the server id by writing _getserver_");
                    return response;
                }

                if (messageText == GetUsersCmd)
                {
                    var conversation = activity.Conversation;
                    await SendReplyAsync(activity, $"Conversation id: {conversation?.Id}, name: {conversation?.Name}");
                    return response;
                }

                if (messageText.Contains("version"))
                {
                    messageText = "1.2";
                }
                await SendReplyAsync(activity, messageText);
            }
            else
            {
                await HandleSystemMessageAsync(activity);
            }
            
            return response;
        }

        private static void TrackMessage(string message)
        {
            var telemetry = new TelemetryClient();
            var trackParams = new Dictionary<string, string>
            {
                {"message", message},
                //{"content", contentString}
            };

            telemetry.TrackEvent("Messages.Post", trackParams);
        }

        private async Task<string> GetServerIdAsync(Activity activity)
        {
            var client = await _repository.GetClientAsync(activity.Conversation.Id, activity.Conversation.Name);
            return client.ServerId;
        }

        private async Task SetServerIdAsync(Activity activity, ServerParams serverParams)
        {
            var serverClient = new ServerClient(serverParams.Id, activity.Conversation.Id)
            {
                UserName = activity.Conversation.Name,
                BotServiceUrl = activity.ServiceUrl,
                BotId = activity.Recipient.Id,
                BotName = activity.Recipient.Name,
                ReplaceFrom = serverParams.ReplaceFrom,
                ReplaceTo = serverParams.ReplaceTo,
            };
            await _repository.SaveServiceClient(serverClient);
            var client = new Client(serverParams.Id, activity.Conversation.Id, activity.Conversation.Name);
            await _repository.SaveClient(client);
        }

        private static async Task SendReplyAsync(Activity activity, string message)
        {
            var connector = GetConnectorClient(activity);            
            var reply = activity.CreateReply(message);
            await connector.Conversations.ReplyToActivityAsync(reply);
        }

        private static ConnectorClient GetConnectorClient(Activity activity)
        {
            var connector = new ConnectorClient(new Uri(activity.ServiceUrl));
            return connector;
        }

        private async Task<Activity> HandleSystemMessageAsync(Activity activity)
        {
            if (activity.Type == ActivityTypes.DeleteUserData)
            {
                // Implement user deletion here
                // If we handle user deletion, return a real message
            }
            else if (activity.Type == ActivityTypes.ConversationUpdate)
            {
                if (activity.MembersAdded?.Any() == true)
                {
                    var client = GetConnectorClient(activity);
                    var dialog = new PromptDialog.PromptChoice<string>(
                        new string[] { "Settings", "Help", "Home Page"}, "Hi, I am TFS bot", "Try again", 1);
                    
                    await Conversation.SendAsync(activity, () => dialog);
                   
                    return null;
                    //await SendReplyAsync(activity, "Hello hello");
                }
                // Handle conversation state changes, like members being added and removed
                // Use Activity.MembersAdded and Activity.MembersRemoved and Activity.Action for info
                // Not available in all channels
            }
            else if (activity.Type == ActivityTypes.ContactRelationUpdate)
            {
                // Handle add/remove from contact lists
                // Activity.From + Activity.Action represent what happened

                await SendReplyAsync(activity, $"Hi I am TFS bot, set up your server by sending message _setserver:<yourserverid>_");
            }
            else if (activity.Type == ActivityTypes.Typing)
            {
                // Handle knowing tha the user is typing
            }
            else if (activity.Type == ActivityTypes.Ping)
            {
            }

            return null;
        }
    }
}