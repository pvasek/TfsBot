using Microsoft.WindowsAzure.Storage.Table;

namespace TfsBot.Common.Entities
{
    public class Client: TableEntity
    {
        public Client(string serverId, string userId, string userName) 
            : base(GetPartitionKey(userId), GetRowKey(userId, userName))
        {
            ServerId = serverId;
        }

        public Client()
        {
        }

        public static string GetPartitionKey(string userId)
        {
            return userId.Substring(0, 1);
        }

        public static string GetRowKey(string userId, string userName)
        {
            return $"{userId}:{userName}";
        }

        public string ServerId { get; set; }
    }
}