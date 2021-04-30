using System.ComponentModel.DataAnnotations;

namespace WebApplication.Controllers
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