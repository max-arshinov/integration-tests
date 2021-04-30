using System.ComponentModel.DataAnnotations;
using WebApplication.Attributes;
using WebApplication.Controllers;

namespace WebApplication.Models
{
    public class Data
    {
        [Required]
        public string A { get; set; }
        
        [Required]
        [BValidation]
        public string B { get; set; }
    }
}