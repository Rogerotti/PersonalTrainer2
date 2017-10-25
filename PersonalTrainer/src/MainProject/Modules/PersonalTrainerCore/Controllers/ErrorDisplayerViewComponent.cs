using Framework.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PersonalTrainerCore.Controllers
{
    /// <summary>
    /// Odpowiada za wyświetlanie błędów logicznych.
    /// </summary>
    public class ErrorDisplayerViewComponent : ViewComponent
    {
        private const String errorDisplayeId = nameof(errorDisplayeId);

        public ErrorDisplayerViewComponent()
        {
        }

        public IViewComponentResult Invoke()
        {
             List<String> errorList = new List<String>();
            var AllKeys = ModelState.Keys;
            var keysEnumerator = AllKeys.GetEnumerator();
            while (keysEnumerator.MoveNext())
            {
                if (keysEnumerator.Current.Contains("AdditionalValidation"))
                {
                    foreach (var item2 in ModelState[keysEnumerator.Current].Errors)
                        errorList.Add(item2.ErrorMessage); 
                    
                }
          
            }
            return View(errorList);
        }
    }
}

