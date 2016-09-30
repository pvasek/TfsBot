using Microsoft.WindowsAzure.Storage.Table;

namespace TfsBot.Common.Entities
{
    public class ServerClient : TableEntity
    {
        public ServerClient(string serverId, string userId) : base(serverId, userId)
        {
        }

        public ServerClient()
        {
        }

        public string ServiceId => PartitionKey;

        public string UserId => RowKey;
        public string UserName { get; set; }

        public string BotServiceUrl { get; set; }        
        public string BotId { get; set; }
        public string BotName { get; set; }
        public string ReplaceFrom { get; set; }
        public string ReplaceTo { get; set; }
    }
}
