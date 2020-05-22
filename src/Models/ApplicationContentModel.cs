namespace VotRomania.Models
{
    public class ApplicationContentModel
    {
        public Language Language { get; set; }
        public string GeneralInfo { get; set; }
        public VotingGuide VotersGuide { get; set; }
    }
}