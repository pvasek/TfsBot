using System;
using System.Threading.Tasks;
using Microsoft.Bot.Connector;
using TfsBot.Common.Entities;

namespace TfsBot.Common.Bot
{
    public static class BotHelper
    {
        public static async Task SendMessageToClient(ServerClient client, string messageText)
        {
            await SendMessageToClient(client, msg => msg.Text = messageText);
        }

        public static async Task SendMessageToClient(ServerClient client, Action<IMessageActivity> setMessage)
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
            var response = await connector.Conversations.SendToConversationAsync((Activity)message);
        }
    }
}
