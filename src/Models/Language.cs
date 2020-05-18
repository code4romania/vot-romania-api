using System.Text.Json.Serialization;

namespace VotRomania.Models
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum Language
    {
        Ro,
        Hu
    }
}