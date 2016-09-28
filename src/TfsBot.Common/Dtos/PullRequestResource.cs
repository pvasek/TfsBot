using System;
using System.Collections.Generic;

namespace TfsBot.Common.Dtos
{
    public class PullRequestResource
    {
        public PullRequestResource()
        {
            Reviewers = new List<Reviewer>();
            Commits = new List<Commit>();
        }
        public string Url { get; set; }
        public Repository Repository { get; set; }
        public int PullRequestId { get; set; }
        public string Status { get; set; }
        public User CreatedBy { get; set; }
        public DateTime? CreationDate { get; set; }
        public DateTime? ClosedDate { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string SourceRefName { get; set; }
        public string TargetRefName { get; set; }
        public string MergeStatus { get; set; }
        public string MergeId { get; set; }
        public Commit LastMergeSourceCommit { get; set; }
        public Commit LastMergeTargetCommit { get; set; }
        public Commit LastMergeCommit { get; set; }
        public IList<Reviewer> Reviewers { get; set; }
        public IList<Commit> Commits { get; set; }
    }
}
