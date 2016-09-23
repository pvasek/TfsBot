using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using Microsoft.Bot.Connector;
using TfsBot.Common.Db;
using TfsBot.Common.Entities;

namespace TfsBot.Controllers
{
    [BotAuthentication]
    public class MessagesController : ApiController
    {
        private const string SetServerCmd = "setserver:";
        private const string GetServerCmd = "getserver";
        private const string HelpCmd = "gethelp";
        private const string GetUsersCmd = "getusers";

        /// <summary>
        /// POST: api/Messages
        /// Receive a message from a user and reply to it
        /// </summary>
        public async Task<HttpResponseMessage> Post([FromBody]Activity activity)
        {
            var response = Request.CreateResponse(HttpStatusCode.OK);
            if (activity.Type == ActivityTypes.Message)
            {
                var messageText = activity.RemoveRecipientMention().Trim();
                if (messageText.StartsWith(SetServerCmd))
                {
                    var serverId = messageText.Substring(SetServerCmd.Length);
                    await SetServerIdAsync(activity, serverId);
                    await SendReplyAsync(activity, $"Your server id was set to '{serverId}'");
                    return response;
                }

                if (messageText == GetServerCmd)
                {
                    var serverId = await GetServerIdAsync(activity);
                    await SendReplyAsync(activity, $"Your server id is: '{serverId}'");
                    return response;
                }
                if (messageText == HelpCmd)
                {
                    await SendReplyAsync(activity, $"You can setup your server id by writing _setserver:<server id>_ or getting the server id by writing _getserver_");
                    return response;
                }
                if (messageText == GetUsersCmd)
                {
                    var conversation = activity.Conversation;
                    await SendReplyAsync(activity, $"Conversation id: {conversation?.Id}, name: {conversation?.Name}");
                    return response;
                }

                if (messageText == "card1")
                {
                    await Card(activity);
                }
                else
                {
                    await SendReplyAsync(activity, messageText);
                }
            }
            else
            {
                await HandleSystemMessageAsync(activity);
            }
            
            return response;
        }

        private async Task<string> GetServerIdAsync(Activity activity)
        {
            var repository = new Repository();
            var client = await repository.GetClientAsync(activity.Conversation.Id, activity.Conversation.Name);
            return client.ServerId;
        }

        private async Task SetServerIdAsync(Activity activity, string serverId)
        {
            var repository = new Repository();
            
            var serverClient = new ServerClient(serverId, activity.Conversation.Id)
            {
                UserName = activity.Conversation.Name,
                BotServiceUrl = activity.ServiceUrl,
                BotId = activity.Recipient.Id,
                BotName = activity.Recipient.Name,
            };
            await repository.SaveServiceClient(serverClient);
            var client = new Client(serverId, activity.Conversation.Id, activity.Conversation.Name);
            await repository.SaveClient(client);
        }

        private static async Task SendReplyAsync(Activity activity, string message)
        {
            var connector = new ConnectorClient(new Uri(activity.ServiceUrl));            
            var reply = activity.CreateReply(message);
            await connector.Conversations.ReplyToActivityAsync(reply);
        }

        private async Task Card(Activity activity)
        {
            var connector = new ConnectorClient(new Uri(activity.ServiceUrl));
            var replyToConversation = activity.CreateReply("Please review:");
            replyToConversation.Recipient = activity.From;
            replyToConversation.Type = "message";
            replyToConversation.Attachments = new List<Attachment>();
            var cardImages = new List<CardImage>();
            cardImages.Add(new CardImage(url: "https://<ImageUrl1>"));
            var cardButtons = new List<CardAction>();
            var plButton = new CardAction()
            {
                Value = "https://tfs.sportresult.com/tfs/FIBA/FIBA%20MAP/_git/MAP/pullrequest/276?view=discussion",
                Type = "openUrl",
                Title = "Open"
            };
            cardButtons.Add(plButton);
            var plCard = new ThumbnailCard()
            {
                Title = "PR277",
                Subtitle = "integration tests for #1378 #1390",
                //Images = cardImages,
                Buttons = cardButtons
            };
            var plAttachment = plCard.ToAttachment();
            replyToConversation.Attachments.Add(plAttachment);
            await connector.Conversations.SendToConversationAsync(replyToConversation);
        }

        private async Task<Activity> HandleSystemMessageAsync(Activity message)
        {
            if (message.Type == ActivityTypes.DeleteUserData)
            {
                // Implement user deletion here
                // If we handle user deletion, return a real message
            }
            else if (message.Type == ActivityTypes.ConversationUpdate)
            {
                // Handle conversation state changes, like members being added and removed
                // Use Activity.MembersAdded and Activity.MembersRemoved and Activity.Action for info
                // Not available in all channels
            }
            else if (message.Type == ActivityTypes.ContactRelationUpdate)
            {
                // Handle add/remove from contact lists
                // Activity.From + Activity.Action represent what happened

                await SendReplyAsync(message, $"Hi I am TFS bot, set up your server by sending message _setserver:<yourserverid>_");
            }
            else if (message.Type == ActivityTypes.Typing)
            {
                // Handle knowing tha the user is typing
            }
            else if (message.Type == ActivityTypes.Ping)
            {
            }

            return null;
        }
    }
}