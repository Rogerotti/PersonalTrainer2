using System;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace Framework.ValidationAttributes
{
    /// <summary>
    /// Atrybut walidacji wzorca hasła.
    /// </summary>
    public class PasswordRegex : ValidationAttribute
    {
        protected override ValidationResult IsValid(Object value, ValidationContext validationContext)
        {
            var password = value as String;
            if(String.IsNullOrWhiteSpace(password)) return new ValidationResult(ErrorMessageString);

            String passwordRegex = @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d).{8,15}$";
            if(!Regex.Match(password, passwordRegex).Success) return new ValidationResult(ErrorMessageString);

            return null;
        }
    }
}
