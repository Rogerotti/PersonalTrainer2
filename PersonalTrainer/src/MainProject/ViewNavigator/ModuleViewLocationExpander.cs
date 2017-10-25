using Microsoft.AspNetCore.Mvc.Razor;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MainProject.ViewNavigator
{
    /// <summary>
    /// Służy do wyszukania dodatkowych widoków. (Pozwala na wyszukiwaniu widoków w modułach)
    /// </summary>
    public class ModuleViewLocationExpander : IViewLocationExpander
    {
        private const String _moduleKey = "module";

        public IEnumerable<String> ExpandViewLocations(ViewLocationExpanderContext context, IEnumerable<String> viewLocations)
        {
            if (context.Values.ContainsKey(_moduleKey))
            {
                var module = context.Values[_moduleKey];
                if (!String.IsNullOrWhiteSpace(module))
                {
                    List<String> moduleViewLocations = new List<String>();
                    moduleViewLocations.Add("/Modules/" + module + ".WebGUI/Views/{1}/{0}.cshtml");
                    moduleViewLocations.Add("/Modules/" + module + ".WebGUI/Views/Shared/{0}.cshtml");
                    if (module != "PersonalTrainerCore")
                        moduleViewLocations.Add("/Modules/" + "PersonalTrainerCore" + ".WebGUI/Views/Shared/{0}.cshtml");

                    viewLocations = moduleViewLocations.Concat(viewLocations);
                }
            }
            return viewLocations;
        }

        /// <summary>
        /// Pobiera nazwę kontrolera który wywołal akcję.
        /// </summary>
        /// <param name="context"></param>
        public void PopulateValues(ViewLocationExpanderContext context)
        {
            var controller = context.ActionContext.ActionDescriptor.DisplayName;
            var moduleName = controller.Split('.')[0];
            if (moduleName != "PersonalTrainer")
            {
                context.Values[_moduleKey] = moduleName;
            }
        }
    }
}
