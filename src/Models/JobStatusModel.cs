using System;
using VotRomania.Stores.Entities;

namespace VotRomania.Models
{
    public class JobStatusModel
    {
        public string JobId { get; set; }
        public JobStatus Status { get; set; }
        public DateTime? Started { get; set; }
        public DateTime? Ended { get; set; }
        public string FileName { get; set; }
    }
}