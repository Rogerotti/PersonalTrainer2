using Framework.Resources;
using System;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace Framework.ValidationAttributes
{
    /// <summary>
    /// Atrybut walidacji wzorca adresu e-mail.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter, AllowMultiple = false)]
    public class EmailRegex : ValidationAttribute
    {
        protected override ValidationResult IsValid(Object value, ValidationContext validationContext)
        {
            var email = value as String;
            if(String.IsNullOrWhiteSpace(email)) return new ValidationResult(ErrorLanguage.EmailEmpty);

            String emailPatern = @"[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?";
            if (!Regex.Match(email, emailPatern).Success) return new ValidationResult(ErrorMessageString);

            return null;
        }
    }
}
