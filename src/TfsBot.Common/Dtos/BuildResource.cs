using System;
using System.Collections.Generic;

namespace TfsBot.Common.Dtos
{
    public class BuildResource
    {
        public BuildResource()
        {
            Requests = new List<BuildResourceRequest>();
        }
        public string Uri { get; set; }
        public int Id { get; set; }
        public string BuildNumber { get; set; }
        public string Url { get; set; }
        public DateTime? StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        public string Reason { get; set; }
        public string Status { get; set; }
        public string DropLocation { get; set; }
        public string SourceGetVersion { get; set; }
        public bool? RetainIndefinitely { get; set; }
        public bool? HasDiagnostics { get; set; }
        public Drop Drop { get; set; }
        public Log Log { get; set; }
        public User LastChangedBy { get; set; }
        public Definition Definition { get; set; }
        public Queue Queue { get; set; }
        public IList<BuildResourceRequest> Requests { get; set; }
    }

    public class Drop
    {
        public string Location { get; set; }
        public string Type { get; set; }
        public string Url { get; set; }
        public string DownloadUrl { get; set; }
    }

    public class Log
    {
        public string Type { get; set; }
        public string Url { get; set; }
        public string DownloadUrl { get; set; }
    }

    public class Definition
    {
        public int BatchSize { get; set; }
        public string TriggerType { get; set; }
        public string DefinitionType { get; set; }
        public int Id { get; set; }
        public string Name { get; set; }
        public string Url { get; set; }
    }

    public class Queue
    {
        public string QueueType { get; set; }
        public int Id { get; set; }
        public string Name { get; set; }
        public string Url { get; set; }
    }

    public class BuildResourceRequest
    {
        public int Id { get; set; }
        public string Url { get; set; }
        public User RequestedFor { get; set; }
    }

}
