using System.Text.Json.Serialization;

namespace VotRomania.Models
{
    public class ApplicationData
    {
        public StaticData[] StaticTexts { get; set; }
        public PollingStationsInfo[] PollingStationsInfo { get; set; }
    }

    public class PollingStationsInfo
    {
        public string id { get; set; }
        public float Lat { get; set; }
        public float Lng { get; set; }
        public PollingStationDetails Properties { get; set; }
    }

    public class StaticData
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

    public class Geometry
    {
        public string type { get; set; }
        public float[] coordinates { get; set; }
    }

    public class PollingStationDetails
    {
        public string Judet { get; set; }
        public string Localitate { get; set; }
        public string NumarSectie { get; set; }
        public string Institutie { get; set; }
        public string Adresa { get; set; }
    }

}
