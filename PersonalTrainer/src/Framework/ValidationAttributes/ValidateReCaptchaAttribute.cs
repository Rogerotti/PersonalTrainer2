using Framework.Resources;
using Microsoft.AspNetCore.Mvc.Filters;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace Framework.ValidationAttributes
{
    public class ValidateReCaptchaAttribute : ActionFilterAttribute
    {
        public const string ReCaptchaModelErrorKey = "ReCaptcha";
        private const string RecaptchaResponseTokenKey = "g-recaptcha-response";
        private const string ApiVerificationEndpoint = "https://www.google.com/recaptcha/api/siteverify";

        public override async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            await DoReCaptchaValidation(context);
            await base.OnActionExecutionAsync(context, next);
        }
        private async Task DoReCaptchaValidation(ActionExecutingContext context)
        {
            if (!context.HttpContext.Request.HasFormContentType)
            {
                AddModelError(context, ErrorLanguage.reCaptchaRequierd);
                return;
            }
            string token = context.HttpContext.Request.Form[RecaptchaResponseTokenKey];
            if (string.IsNullOrWhiteSpace(token))
            {
                AddModelError(context, ErrorLanguage.reCaptchaRequierd);
            }
            else
            {
                await ValidateRecaptcha(context, token);
            }
        }
        private static void AddModelError(ActionExecutingContext context, string error)
        {
            context.ModelState.AddModelError(ReCaptchaModelErrorKey, error.ToString());
        }
        private async Task ValidateRecaptcha(ActionExecutingContext context, string token)
        {
            var secret = "6LeBIUAUAAAAAKrLXCvJLqfF1vf6_RX8rk40-9KS";
            using (var webClient = new HttpClient())
            {
                var content = new FormUrlEncodedContent(new[]
                {
                        new KeyValuePair<string, string>("secret", secret),
                        new KeyValuePair<string, string>("response", token)
                    });
                HttpResponseMessage response = await webClient.PostAsync(ApiVerificationEndpoint, content);
                string json = await response.Content.ReadAsStringAsync();
                var reCaptchaResponse = JsonConvert.DeserializeObject<ReCaptchaResponse>(json);
                if (reCaptchaResponse == null)
                {
                    AddModelError(context, ErrorLanguage.reCaptchaRequierd);
                }
                else if (!reCaptchaResponse.success)
                {
                    AddModelError(context, ErrorLanguage.reCaptchaRequierd);
                }
            }
        }
    }
    public class ReCaptchaResponse
    {
        public bool success { get; set; }
        public string challenge_ts { get; set; }
        public string hostname { get; set; }
        public string[] errorcodes { get; set; }
    }
}
