using System.ComponentModel.DataAnnotations;

namespace WebApplication.Controllers
{
    public class BValidationAttribute: ValidationAttribute
    {
        public override bool IsValid(object value)
        {
            if (value?.Equals("B") != true)
            {
                return false;
            }

            return true;
        }
    }
}