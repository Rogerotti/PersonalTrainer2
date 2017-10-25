using System;
using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace Framework.ValidationAttributes
{
    /// <summary>
    /// Atrybut walidacji potwierdzenia hasła.
    /// </summary>
    public class PasswordConfirmation : ValidationAttribute
    {
        private readonly String otherPasswordName;

        public PasswordConfirmation(String otherPasswordName)
        {
            this.otherPasswordName = otherPasswordName;
        }

        protected override ValidationResult IsValid(Object value, ValidationContext validationContext)
        {
            var password = value as String;
            if(String.IsNullOrWhiteSpace(password)) return new ValidationResult(ErrorMessageString);

            var otherPasswordInformation = validationContext.ObjectType.GetProperty(otherPasswordName);
            if (otherPasswordInformation == null)
            {
                return new ValidationResult(
                    String.Format("Unknown property: {0}", nameof(otherPasswordName))
                );
            }
            var otherPassword = (String)otherPasswordInformation.GetValue(validationContext.ObjectInstance, null);
            if (String.IsNullOrWhiteSpace(otherPassword)) return null;

            if (password.CompareTo(otherPassword) != 0)
                return new ValidationResult(ErrorMessageString);

            return null;
        }
    }
}
