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
            var connector = new ConnectorClient(new Uri(client.BotServiceUrl), new MicrosoftAppCredentials());
            var userAccount = new ChannelAccount(name: client.UserName, id: client.UserId);
            var botAccount = new ChannelAccount(name: client.BotName, id: client.BotId);
            //var conversationId = await connector.Conversations.CreateDirectConversationAsync(botAccount, userAccount);
            var message = Activity.CreateMessageActivity();
            message.From = botAccount;
            message.Recipient = userAccount;
            message.Conversation = new ConversationAccount(false, client.UserId);//conversationId.Id);
            message.Locale = "en-Us";
            if (client.ReplaceFrom != null && client.ReplaceTo != null)
            {
                messageText = messageText.Replace(client.ReplaceFrom, client.ReplaceTo);
            }
            message.Text = messageText;
            await connector.Conversations.SendToConversationAsync((Activity) message);
        }
    }
}
