﻿using eCommerce.Data;
using eCommerce.Entities;
using eCommerce.Services;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;
using Microsoft.Owin.Security.Cookies;
using Owin;
using System;

namespace eCommerce.Web
{
    public partial class Startup
    {
        // For more information on configuring authentication, please visit https://go.microsoft.com/fwlink/?LinkId=301864
        public void ConfigureAuth(IAppBuilder app)
        {
            // Configure the db context, user manager and signin manager to use a single instance per request
            app.CreatePerOwinContext(eCommerceContext.Create);
            app.CreatePerOwinContext<eCommerceUserManager>(eCommerceUserManager.Create);
            app.CreatePerOwinContext<eCommerceSignInManager>(eCommerceSignInManager.Create);
            app.CreatePerOwinContext<eCommerceRoleManager>(eCommerceRoleManager.Create);

            // Enable the eCommerce to use a cookie to store information for the signed in user
            // and to use a cookie to temporarily store information about a user logging in with a third party login provider
            // Configure the sign in cookie
            app.UseCookieAuthentication(new CookieAuthenticationOptions
            {
                AuthenticationType = DefaultAuthenticationTypes.ApplicationCookie,
                LoginPath = new PathString("/login"),
                Provider = new CookieAuthenticationProvider
                {
                    // Enables the eCommerce to validate the security stamp when the user logs in.
                    // This is a security feature which is used when you change a password or add an external login to your account.  
                    OnValidateIdentity = SecurityStampValidator.OnValidateIdentity<eCommerceUserManager, eCommerceUser>(
                        validateInterval: TimeSpan.FromMinutes(30),
                        regenerateIdentity: (manager, user) => user.GenerateUserIdentityAsync(manager))
                }
            });
            app.UseExternalSignInCookie(DefaultAuthenticationTypes.ExternalCookie);

            // Enables the eCommerce to temporarily store user information when they are verifying the second factor in the two-factor authentication process.
            app.UseTwoFactorSignInCookie(DefaultAuthenticationTypes.TwoFactorCookie, TimeSpan.FromMinutes(5));

            // Enables the eCommerce to remember the second login verification factor such as phone or email.
            // Once you check this option, your second step of verification during the login process will be remembered on the device where you logged in from.
            // This is similar to the RememberMe option when you log in.
            app.UseTwoFactorRememberBrowserCookie(DefaultAuthenticationTypes.TwoFactorRememberBrowserCookie);

            //// Uncomment the following lines to enable logging in with third party login providers
            //app.UseMicrosoftAccountAuthentication(
            //    clientId: "4f34f34f34f3",
            //    clientSecret: "34f34f34f34f34");

            //app.UseTwitterAuthentication(
            //   consumerKey: "dtcfyvgubhinjomkonibuvycrt",
            //   consumerSecret: "ycvubhijnokmplpmonuibyvtcr5estrcyvubino");

            //app.UseFacebookAuthentication(
            //   appId: "543658569819344",
            //   appSecret: "88f343050cf950829247b87836e95c66");

            //app.UseGoogleAuthentication(new GoogleOAuth2AuthenticationOptions()
            //{
            //    ClientId = "4cr34rc34rc34rc3",
            //    ClientSecret = "34rc34cr34rc34cr34rc4r34rc34rc"
            //});
        }
    }
}