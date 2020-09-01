using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace VotRomania.Models
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum Language
    {
        [EnumMember(Value = "unknown")] Unknown = 0,
        [EnumMember(Value = "Ro")] Ro = 1,
        [EnumMember(Value = "Hu")] Hu = 2,
        [EnumMember(Value = "En")] En = 3
    }
}