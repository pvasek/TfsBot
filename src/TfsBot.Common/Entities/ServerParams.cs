using System;
using System.Text;

namespace TfsBot.Common.Entities
{
    public class ServerParams
    {
        public string Id { get; set; }
        public string ReplaceFrom { get; set; }
        public string ReplaceTo { get; set; }

        public static ServerParams Parse(string text)
        {
            var parts = text.Split(';');
            return new ServerParams
            {
                Id = parts[0],
                ReplaceFrom = parts.Length == 3 ? parts[1] : null,
                ReplaceTo = parts.Length == 3 ? parts[2] : null,
            };
        }

        public const string keys = "abcdefghijklmnopqrstuvwxyz0123456789";

        public static ServerParams New(string prefix = "")
        {
            var random = new Random(Environment.TickCount);
            var id = new StringBuilder(prefix);
            for (var i = 0; i < 30; i++)
            {
                id.Append(keys[random.Next(0, keys.Length - 1)]);
            }
            return new ServerParams {Id = id.ToString()};
        }

        public override string ToString()
        {
            return ReplaceTo == null ? Id : $"{Id} (replace from: {ReplaceFrom}, replace to: {ReplaceTo})";
        }
    }
}
