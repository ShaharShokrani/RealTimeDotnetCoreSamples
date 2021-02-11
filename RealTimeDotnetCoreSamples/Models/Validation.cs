using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace RealTimeDotnetCoreSamples.Models
{
    public interface IValidation
    {
        bool IsValid { get; }
        string ValidationError { get; }
        IEnumerable<ValidationResult> Validate();
    }

    public abstract class Validation : IValidation
    {
        public bool IsValid => !this.Validate().Any();
        public string ValidationError => this.IsValid ? null : string.Join(";", this.Validate());
        public abstract IEnumerable<ValidationResult> Validate();
    }
}
