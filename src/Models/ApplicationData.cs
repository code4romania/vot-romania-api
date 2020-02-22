using System.Text.Json.Serialization;

namespace VotRomania.Models
{
    public class ApplicationData
    {
        public Language Language { get; set; }
        public string GeneralInfo { get; set; }
        public VotingGuide VotersGuide { get; set; }
    }

    public class VotingGuide
    {
        public string Description { get; set; }
        public Option[] Options { get; set; }
    }

    public class Option
    {
        public string Title { get; set; }
        public string Description { get; set; }
    }

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum Language
    { 
         Ro,
         Hu,
    }
}
