namespace TfsBot.Common.Dtos
{
    public class PullRequest: Request
    {      
        public PullRequestResource Resource { get; set; }
        public string ResourceVersion { get; set; }
        public string Scope { get; set; }
    }
}
