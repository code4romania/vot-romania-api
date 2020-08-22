
using System;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace VotRomania.Stores.Entities
{
    public class UploadJobsEntity
    {
        public int Id { get; set; }
        public string JobId { get; set; }
        public string FileName { get; set; }
        public string Base64File { get; set; }
        public JobStatus JobStatus { get; set; }
        public DateTime? Started { get; set; }
        public DateTime? Ended { get; set; }
    }

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum JobStatus
    {
        [EnumMember(Value = "notStarted")] NotStarted = 1,
        [EnumMember(Value = "started")] Started = 2,
        [EnumMember(Value = "finished")] Finished = 3,
        [EnumMember(Value = "canceled")] Canceled = 4,
        [EnumMember(Value = "imported")] Imported = 5,
    }
}