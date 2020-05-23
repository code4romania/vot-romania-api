using System.ComponentModel.DataAnnotations;

namespace VotRomania.Models
{
    public class TokenRequestModel
    {
        [Required] public string UserName { get; set; }
        [Required] public string Password { get; set; }
    }
}