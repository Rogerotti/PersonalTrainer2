using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;

namespace PersonalTrainerCore.Controllers
{
    /// <summary>
    /// Odpowiada za wyświetlanie błędów logicznych.
    /// </summary>
    public class ErrorDisplayerViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke()
        {
             List<String> errorList = new List<String>();
            var AllKeys = ModelState.Keys;
            var keysEnumerator = AllKeys.GetEnumerator();
            while (keysEnumerator.MoveNext())
            {
                if (keysEnumerator.Current.Contains("AdditionalValidation"))
                {
                    foreach (var item in ModelState[keysEnumerator.Current].Errors)
                        errorList.Add(item.ErrorMessage); 
                }
          
            }
            return View(errorList);
        }
    }
}

