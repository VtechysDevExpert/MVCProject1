#pragma warning disable CS0234 // The type or namespace name 'Entities' does not exist in the namespace 'eCommerce' (are you missing an assembly reference?)
using eCommerce.Entities;
#pragma warning restore CS0234 // The type or namespace name 'Entities' does not exist in the namespace 'eCommerce' (are you missing an assembly reference?)
using System;
using System.Linq;
using System.Web;
using System.Web.WebPages;

namespace eCommerce.Shared.Helpers
{
    public static class AppDataHelper
    {
        private const string IS_INITILIZED = "IS_INITILIZED";
        private const string CURRENT_LANGUAGE = "CURRENT_LANGUAGE";
        private const string IS_RTL = "IS_RTL";

        public static void Populate()
        {
            // TODO: Do we need to make this method thread-safe?
            if (IsIntitialized)
            {
                return;
            }

            #region Get the requested Language OR set 'en' as current if nothing specified in URL
            string languageShortName = SharedURLHelper.GetLanguageUrlComponent();

            var language = LanguagesHelper.EnabledLanguages.FirstOrDefault(x => x.ShortCode == languageShortName);

            var forceUpdate = false;

            if (language == null)
            {
                CurrentLanguage = LanguagesHelper.DefaultLanguage;
                forceUpdate = true;
            }
            else CurrentLanguage = language;

            if (CurrentLanguage == null)
            {
                throw new Exception("NO LANGUAGE DETECTED");
            }

            SharedURLHelper.TryAddRouteKeyValue("lang", CurrentLanguage.ShortCode, forceUpdate);

            IsRTL = CurrentLanguage.IsRTL;
            #endregion

            IsIntitialized = true;
        }

        public static bool IsIntitialized
        {
            get
            {
                bool retVal = false;
                if (HttpContext.Current.Items[IS_INITILIZED] != null)
                {
                    retVal = (bool)HttpContext.Current.Items[IS_INITILIZED];
                }
                return retVal;
            }
            set
            {
                HttpContext.Current.Items[IS_INITILIZED] = value;
            }

        }

#pragma warning disable CS0246 // The type or namespace name 'Language' could not be found (are you missing a using directive or an assembly reference?)
        public static Language CurrentLanguage
#pragma warning restore CS0246 // The type or namespace name 'Language' could not be found (are you missing a using directive or an assembly reference?)
        {
            get
            {
                return (Language)HttpContext.Current.Items[CURRENT_LANGUAGE];
            }
            set
            {
                HttpContext.Current.Items[CURRENT_LANGUAGE] = value;
            }
        }

        public static bool IsRTL
        {
            get
            {
                bool retVal = false;
                if (HttpContext.Current.Items[IS_RTL] != null)
                {
                    retVal = (bool)HttpContext.Current.Items[IS_RTL];
                }
                return retVal;
            }
            set
            {
                HttpContext.Current.Items[IS_RTL] = value;
            }
        }

        public static bool IsMobile
        {
            get
            {
                try
                {
                    return new HttpContextWrapper(HttpContext.Current).GetOverriddenBrowser().IsMobileDevice && !IsTablet;
                }
                catch
                {
                    return false;
                }
            }
        }

        public static bool IsTablet
        {
            get
            {
                try
                {
                    var userAgent = HttpContext.Current.Request.UserAgent.ToLower().Trim();

                    return !string.IsNullOrEmpty(userAgent) && userAgent.Contains("ipad");
                }
                catch
                {
                    return false;
                }
            }
        }
    }
}
