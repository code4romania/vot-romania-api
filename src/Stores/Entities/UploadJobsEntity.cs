
namespace VotRomania.Stores.Entities
{
    public class UploadJobsEntity
    {
        public int Id { get; set; }
        public string JobId { get; set; }
        public string FileName { get; set; }
        public string Base64File { get; set; }
        public JobStatus JobStatus { get; set; }
    }

    public enum JobStatus
    {
        NotStarted = 1,
        Started = 2,
        Finished = 3,
        Failed = 4
    }
}