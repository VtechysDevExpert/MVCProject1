﻿#pragma warning disable CS0234 // The type or namespace name 'Entities' does not exist in the namespace 'eCommerce' (are you missing an assembly reference?)
using eCommerce.Entities;
#pragma warning restore CS0234 // The type or namespace name 'Entities' does not exist in the namespace 'eCommerce' (are you missing an assembly reference?)
using eCommerce.Shared.Extensions;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace eCommerce.Shared.Helpers
{
    public static class LocalizationHelper
    {
        public static ConcurrentDictionary<string, HtmlString> ResourcesDictionary { get; set; }

#pragma warning disable CS0246 // The type or namespace name 'LanguageResource' could not be found (are you missing a using directive or an assembly reference?)
        public static void LoadResourceLocalizations(List<LanguageResource> list)
#pragma warning restore CS0246 // The type or namespace name 'LanguageResource' could not be found (are you missing a using directive or an assembly reference?)
        {
            if (list != null && list.Count > 0)
            {
                ResourcesDictionary = new ConcurrentDictionary<string, HtmlString>(list.Distinct(new LanguageResourceComparer()).ToDictionary(x => x.Key.SafeTrim(), x => new HtmlString(x.Value.SafeTrim())));
            }
            else ResourcesDictionary = new ConcurrentDictionary<string, HtmlString>();
        }

        public static HtmlString GetLocalizedString(string resourceKey, int languageID)
        {
            HtmlString htmlString = null;

            ResourcesDictionary.TryGetValue(string.Format("{0}_{1}", languageID, resourceKey), out htmlString);

            if (htmlString == null)
            {
                var key = resourceKey.Contains(".") ? resourceKey.Substring(resourceKey.LastIndexOf('.')).Replace(".", "") : resourceKey;

                htmlString = new HtmlString(key.MakeWord());

                //throw new Exception($"resource missing: {resourceKey}");
            }

            return htmlString;
        }

        public static HtmlString Localized(this string resourceKey, int languageID)
        {
            return GetLocalizedString(resourceKey, languageID);
        }

        public static HtmlString Localized(this string resourceKey)
        {
            return GetLocalizedString(resourceKey, AppDataHelper.CurrentLanguage.ID);
        }

        public static string LocalizedString(this string resourceKey)
        {
            return GetLocalizedString(resourceKey, AppDataHelper.CurrentLanguage.ID).ToString();
        }
    }

#pragma warning disable CS0246 // The type or namespace name 'LanguageResource' could not be found (are you missing a using directive or an assembly reference?)
    public class LanguageResourceComparer : IEqualityComparer<LanguageResource>
#pragma warning restore CS0246 // The type or namespace name 'LanguageResource' could not be found (are you missing a using directive or an assembly reference?)
    {
#pragma warning disable CS0246 // The type or namespace name 'LanguageResource' could not be found (are you missing a using directive or an assembly reference?)
#pragma warning disable CS0246 // The type or namespace name 'LanguageResource' could not be found (are you missing a using directive or an assembly reference?)
        public bool Equals(LanguageResource x, LanguageResource y)
#pragma warning restore CS0246 // The type or namespace name 'LanguageResource' could not be found (are you missing a using directive or an assembly reference?)
#pragma warning restore CS0246 // The type or namespace name 'LanguageResource' could not be found (are you missing a using directive or an assembly reference?)
        {
            // Two items are equal if their keys are equal.
            return x.Key == y.Key;
        }

#pragma warning disable CS0246 // The type or namespace name 'LanguageResource' could not be found (are you missing a using directive or an assembly reference?)
        public int GetHashCode(LanguageResource obj)
#pragma warning restore CS0246 // The type or namespace name 'LanguageResource' could not be found (are you missing a using directive or an assembly reference?)
        {
            return obj.Key.GetHashCode();
        }
    }
}
