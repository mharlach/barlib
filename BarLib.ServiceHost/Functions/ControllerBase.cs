using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using System.Linq;

namespace BarLib.ServiceHost.Functions
{
    public abstract class ControllerBase
    {
        protected bool TryValidateObject(IValidatableObject obj, out IEnumerable<ValidationResult> validationResults)
        {
            validationResults = new List<ValidationResult>();
            var tmp = obj.Validate(new ValidationContext(obj));
            validationResults = tmp;

            return !tmp.Any();
        }
    }
}
