using System.Text.Json.Serialization;
using Newtonsoft.Json;

namespace VotRomania.Models
{
    public class ApplicationData
    {
        public StaticData[] StaticTexts { get; set; }
        public PollingStationsInfo[] PollingStationsInfo { get; set; }
    }

    public class PollingStationsInfo
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("lat")]
        public float Lat { get; set; }

        [JsonProperty("lng")]
        public float Lng { get; set; }

        [JsonProperty("properties")]
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

    [System.Text.Json.Serialization.JsonConverter(typeof(JsonStringEnumConverter))]
    public enum Language
    {
        Ro,
        Hu
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