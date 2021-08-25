using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace EUC_form.Helpers
{
    public static class HelperFunctions
    {
        static public List<SelectListItem> GetCountryList()
        {
            var culture
                = CultureInfo.GetCultures(CultureTypes.AllCultures & ~CultureTypes.NeutralCultures);

            List<SelectListItem> result = new List<SelectListItem>();

            for (int i = 0; i < culture.Length; i++)
            {
                result.Add(new SelectListItem
                {
                    Text = culture[i].EnglishName,
                    Value = i.ToString()
                });
            }
            return result;
        }
    }
}
