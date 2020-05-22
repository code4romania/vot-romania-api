using System.ComponentModel.DataAnnotations.Schema;
using Newtonsoft.Json;
using VotRomania.Models;

namespace VotRomania.Stores.Entities
{
    public class ApplicationContentEntity
    {
        public Language Language { get; set; }
        public string Data { get; set; }

        [NotMapped]
        public ApplicationContentModel ApplicationContent
        {
            get
            {
                return Data == null ? null : JsonConvert.DeserializeObject<ApplicationContentModel>(Data);
            }
            set
            {
                if (value != null)
                {
                    Data = JsonConvert.SerializeObject(value);
                    Language = value.Language;
                }
                else
                {
                    Data = null;
                }
            }
        }
    }
}