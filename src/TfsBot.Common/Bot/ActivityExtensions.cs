using Microsoft.Bot.Connector;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace TfsBot.Common.Bot
{
    public static class ActivityExtensions
    {
        public static string RemoveFuzzyRecipientMention(this Activity activity)
        {
            var result = activity.RemoveRecipientMention();
            var id = activity.Recipient.Id;
            var mentionedText = activity
                .GetMentions()
                .FirstOrDefault(i => i.Mentioned.Id == id)?
                .Text;
            mentionedText = StripOutTags(mentionedText);

            if (mentionedText == null)
            {
                return result;
            }

            return Regex.Replace(result, mentionedText, "", RegexOptions.IgnoreCase);
        }

        private static string StripOutTags(string text)
        {
            if (text == null)
            {
                return null;
            }

            var result = new StringBuilder();
            var inTag = false;
            foreach (var ch in text)
            {
                if (ch == '<')
                {
                    inTag = true;
                }
                if (!inTag)
                {
                    result.Append(ch);
                }
                if (ch == '>')
                {
                    inTag = false;
                }
            }
            return result.ToString();
        }
    }
}
