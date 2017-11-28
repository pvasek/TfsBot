using System;
using System.Collections.Generic;
using System.Globalization;
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
using TfsBot.Common.Bot;
using AdaptiveCards;

namespace TfsBot.Controllers
{
    [BotAuthentication]
    [Route("api/botmessages")]
    public class BotMessagesController : ApiController
    {
        public BotMessagesController(IRepository repository, Configuration configuration)
        {
            _repository = repository;
            _configuration = configuration;
        }

        private readonly IRepository _repository;
        private readonly Configuration _configuration;


        [HttpPost]
        public async Task<HttpResponseMessage> Post([FromBody]Activity activity)
        {
            var response = Request.CreateResponse(HttpStatusCode.OK);
            if (activity.Type == ActivityTypes.Message)
            {
                var mentions = activity.GetMentions().FirstOrDefault(i => i.Mentioned.Id == activity.Recipient.Id);
                var messageText = activity.RemoveFuzzyRecipientMention().Trim();                
                var messageTextLower = messageText.ToLowerInvariant();

                if (messageTextLower.Contains("testcard1"))
                {
                    Activity replyToConversation = activity.CreateReply();
                    replyToConversation.Attachments = new List<Attachment>();
                    var card = new AdaptiveCard();

                    card.Speak = "<s>Your  meeting about \"Adaptive Card design session\"<break strength='weak'/> is starting at 12:30pm</s><s>Do you want to snooze <break strength='weak'/> or do you want to send a late notification to the attendees?</s>";

                    // Add text to the card.
                    card.Body.Add(new AdaptiveTextBlock()
                    {
                        Text = "TFS bot setup",
                        Size = AdaptiveTextSize.Large,
                        Weight = AdaptiveTextWeight.Bolder
                    });

                    // Add text to the card.
                    card.Body.Add(new AdaptiveTextBlock()
                    {
                        Text = "In order to use TFS bot you need fill out the following form.",
                        Wrap = true,
                    });

                    // Add text to the card.
                    card.Body.Add(new AdaptiveTextBlock()
                    {
                        Text = "All values are stored encrypted. You will receive your private key after finishing the setup.",
                        Wrap = true,
                        IsSubtle = true
                    });

                    card.Body.Add(new AdaptiveTextInput
                    {
                        Id = "serverUrl",
                        Placeholder = "Server URL",
                        Value = "",
                    });

                    card.Body.Add(new AdaptiveTextInput
                    {
                        Id = "apiKey",
                        Placeholder = "API Key",
                        Value = "",
                    });

                    card.Actions.Add(new AdaptiveSubmitAction
                    {
                        Id = "tfs_bot_initial_setup",
                        Title = "Submit",
                    });

                    // Create the attachment.
                    Attachment attachment = new Attachment()
                    {
                        ContentType = AdaptiveCard.ContentType,
                        Content = card
                    };

                    replyToConversation.Attachments.Add(attachment);

                    var connector = new ConnectorClient(new Uri(activity.ServiceUrl));                    
                    var reply = await connector.Conversations.SendToConversationAsync(replyToConversation);
    
                }
            }          

            return response;
        }
    
    }
}