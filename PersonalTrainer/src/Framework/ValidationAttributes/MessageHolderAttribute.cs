using System;
using System.ComponentModel.DataAnnotations;

namespace Framework.ValidationAttributes
{

    class MessageHolderAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(Object value, ValidationContext validationContext)
        {
            return null;
        }
    }
}
