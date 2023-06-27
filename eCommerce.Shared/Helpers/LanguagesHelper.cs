#pragma warning disable CS0234 // The type or namespace name 'Entities' does not exist in the namespace 'eCommerce' (are you missing an assembly reference?)
using eCommerce.Entities;
#pragma warning restore CS0234 // The type or namespace name 'Entities' does not exist in the namespace 'eCommerce' (are you missing an assembly reference?)
using System.Collections.Generic;
using System.Linq;

namespace eCommerce.Shared.Helpers
{
    public static class LanguagesHelper
    {
        /// <summary>
        /// All languages that are marked true for isEnabled property in database.
        /// </summary>
#pragma warning disable CS0246 // The type or namespace name 'Language' could not be found (are you missing a using directive or an assembly reference?)
        public static List<Language> EnabledLanguages { get; set; }
#pragma warning restore CS0246 // The type or namespace name 'Language' could not be found (are you missing a using directive or an assembly reference?)

        /// <summary>
        /// All languages that are marked true for isEnabled property in database.
        /// </summary>
#pragma warning disable CS0246 // The type or namespace name 'Language' could not be found (are you missing a using directive or an assembly reference?)
        public static List<Language> LanguagesWithResources
        {
#pragma warning restore CS0246 // The type or namespace name 'Language' could not be found (are you missing a using directive or an assembly reference?)
            get
            {
                return EnabledLanguages != null && EnabledLanguages.Count > 0 ? EnabledLanguages.Where(x => LanguageIDsWithResources.Contains(x.ID)).ToList() : null;
            }
        }
        private static List<int> LanguageIDsWithResources { get; set; }

        /// <summary>
        /// A language that is marked true for IsDefault property in database.
        /// </summary>
#pragma warning disable CS0246 // The type or namespace name 'Language' could not be found (are you missing a using directive or an assembly reference?)
        public static Language DefaultLanguage { get; set; }
#pragma warning restore CS0246 // The type or namespace name 'Language' could not be found (are you missing a using directive or an assembly reference?)

        /// <summary>
        /// English Langauge Shortcode used for product detail title sanitization
        /// </summary>
        public static string EnglishLanguageShortCode = "en";

#pragma warning disable CS0246 // The type or namespace name 'Language' could not be found (are you missing a using directive or an assembly reference?)
#pragma warning disable CS0246 // The type or namespace name 'Language' could not be found (are you missing a using directive or an assembly reference?)
        public static void LoadLanguages(List<Language> enabledLanguages, Language defaultLanguage, List<int> languageIDsWithResources)
#pragma warning restore CS0246 // The type or namespace name 'Language' could not be found (are you missing a using directive or an assembly reference?)
#pragma warning restore CS0246 // The type or namespace name 'Language' could not be found (are you missing a using directive or an assembly reference?)
        {
            EnabledLanguages = enabledLanguages;

            DefaultLanguage = defaultLanguage;

            LanguageIDsWithResources = languageIDsWithResources;
        }
    }
}
