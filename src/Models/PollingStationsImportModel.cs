using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;
using VotRomania.Attributes;

namespace VotRomania.Models
{
    public class PollingStationsImportModel
    {
        [Required(ErrorMessage = "Please select a model.")]
        [DataType(DataType.Upload)]
        [AllowedExtensions(new[] { ".xlsx" })]
        public IFormFile FormFile { get; set; }
    }
}