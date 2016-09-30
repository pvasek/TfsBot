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

        public override string ToString()
        {
            return ReplaceTo == null ? Id : $"{Id} (replace from: {ReplaceFrom}, replace to: {ReplaceTo})";
        }
    }
}
