using System;

namespace TfsBot.Common.Dtos
{
    public class Request
    {
        public string SubscriptionId { get; set; }
        public int NotificationId { get; set; }
        public string Id { get; set; }
        public string EventType { get; set; }
        public string PublisherId { get; set; }
        public Message Message { get; set; }
        public Message DetailedMessage { get; set; }
        public DateTime? CreatedDate { get; set; }
    }
}
