using eCommerce.Entities;
using eCommerce.Services;
using eCommerce.Shared.Helpers;
using eCommerce.Web.ViewModels;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using System;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace eCommerce.Web.Controllers
{
    [Authorize]
    public class UsersController : PublicBaseController
    {
        #region Constructor and Managers
        private eCommerceSignInManager _signInManager;
        private eCommerceUserManager _userManager;
        private eCommerceRoleManager _roleManager;

        public eCommerceSignInManager SignInManager
        {
            get
            {
                return _signInManager ?? HttpContext.GetOwinContext().Get<eCommerceSignInManager>();
            }
            private set
            {
                _signInManager = value;
            }
        }

        public eCommerceUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<eCommerceUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }

        public eCommerceRoleManager RoleManager
        {
            get
            {
                return _roleManager ?? HttpContext.GetOwinContext().Get<eCommerceRoleManager>();
            }
            private set
            {
                _roleManager = value;
            }
        }

        private IAuthenticationManager AuthenticationManager
        {
            get
            {
                return HttpContext.GetOwinContext().Authentication;
            }
        }

        public UsersController()
        {
        }

        public UsersController(eCommerceUserManager userManager, eCommerceSignInManager signInManager, eCommerceRoleManager roleManager)
        {
            UserManager = userManager;
            SignInManager = signInManager;
            RoleManager = roleManager;
        }

        #endregion

        [HttpGet]
        [AllowAnonymous]
        [OutputCache(Duration = 0)]
        public ActionResult Register()
        {
            return View(new RegisterViewModel());
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<JsonResult> Register(RegisterViewModel model)
        {
            JsonResult jsonResult = new JsonResult();
            var user_id = UserProfileService.Instance.GetUserId();
            var user = new eCommerceUser { FullName = model.FullName, UserName = model.Username, Email = model.Email, RegisteredOn = DateTime.Now,UserId= user_id };
            var result = await UserManager.CreateAsync(user, model.Password);

            if (result.Succeeded)
            {
                if (await RoleManager.RoleExistsAsync("User"))
                {
                    //assign User role to newly registered user
                    await UserManager.AddToRoleAsync(user.Id, "User");
                }

                await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);

                // For more information on how to enable account confirmation and password reset please visit https://go.microsoft.com/fwlink/?LinkID=320771
                // Send an email with this link
                // string code = await UserManager.GenerateEmailConfirmationTokenAsync(user.Id);
                // var callbackUrl = Url.Action("ConfirmEmail", "Account", new { userId = user.Id, code = code }, protocol: Request.Url.Scheme);
                // await UserManager.SendEmailAsync(user.Id, "Confirm your account", "Please confirm your account by clicking <a href=\"" + callbackUrl + "\">here</a>");                

                await UserManager.SendEmailAsync(user.Id, EmailTextHelpers.AccountRegisterEmailSubject(AppDataHelper.CurrentLanguage.ID), EmailTextHelpers.AccountRegisterEmailBody(AppDataHelper.CurrentLanguage.ID, Url.Action("Login", "Users", null, protocol: Request.Url.Scheme)));

                jsonResult.Data = new { Success = true };
            }
            else
            {
                jsonResult.Data = new { Success = false, Messages = string.Join("<br />", result.Errors) };
            }

            return jsonResult;
        }

        [HttpGet]
        [AllowAnonymous]
        [OutputCache(Duration = 0)]
        public ActionResult Login(string returnUrl)
        {
            return View(new LoginViewModel() { ReturnUrl = returnUrl });
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<JsonResult> Login(LoginViewModel model)
        {
            JsonResult jsonResult = new JsonResult();

            var result = await SignInManager.PasswordSignInAsync(model.Username, model.Password, model.RememberMe, shouldLockout: false);

            switch (result)
            {
                case SignInStatus.Success:
                    jsonResult.Data = new { Success = true, RequiresVerification = false };
                    break;
                case SignInStatus.RequiresVerification:
                    jsonResult.Data = new { Success = true, RequiresVerification = true };
                    break;
                case SignInStatus.LockedOut:
                    jsonResult.Data = new { Success = false, Messages = "PP.Login.Validation.LockedOut".LocalizedString() };
                    break;
                case SignInStatus.Failure:
                default:
                    jsonResult.Data = new { Success = false, Messages = "PP.Login.Validation.InvalidLoginAttempt".LocalizedString() };
                    break;
            }

            return jsonResult;
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult SocialLogin(string provider, string returnUrl)
        {
            // Request a redirect to the external login provider
            return new ChallengeResult(provider, Url.SocialLoginCallback());
        }

        [AllowAnonymous]
        public async Task<ActionResult> SocialLoginCallback(string returnUrl)
        {
            var loginInfo = await AuthenticationManager.GetExternalLoginInfoAsync();
            if (loginInfo == null)
            {
                return Redirect(Url.Login(returnUrl));
            }

            // Sign in the user with this external login provider if the user already has a login
            var result = await SignInManager.ExternalSignInAsync(loginInfo, isPersistent: false);
            switch (result)
            {
                case SignInStatus.Success:
                    return Redirect(Url.Home());
                case SignInStatus.LockedOut:
                    return View("Lockout");
                case SignInStatus.RequiresVerification:
                    return RedirectToAction("SendCode", new { ReturnUrl = returnUrl, RememberMe = false });
                case SignInStatus.Failure:
                default:
                    // If the user does not have an existing account, then create an account
                    var user = new eCommerceUser { UserName = loginInfo.DefaultUserName, Email = loginInfo.Email };
                    var createUserResult = await UserManager.CreateAsync(user);
                    if (createUserResult.Succeeded)
                    {
                        createUserResult = await UserManager.AddLoginAsync(user.Id, loginInfo.Login);
                        if (createUserResult.Succeeded)
                        {
                            await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
                            return Redirect(Url.Login(returnUrl));
                        }
                        else return View("Error");
                    }
                    else return View("Error");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LogOff()
        {
            AuthenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);

            return Redirect(Url.Home());
        }

        [AllowAnonymous]
        public ActionResult ForgotPassword()
        {
            return View(new ForgotPasswordViewModel());
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<JsonResult> ForgotPassword(ForgotPasswordViewModel model)
        {
            var user = !string.IsNullOrEmpty(model.Username) ? await UserManager.FindByNameAsync(model.Username) : null;

            if (user != null)
            {
                string code = await UserManager.GeneratePasswordResetTokenAsync(user.Id);

                var callbackUrl = Url.Action("ResetPassword", "Users", new { userId = user.Id, code = code }, protocol: Request.Url.Scheme);

                await UserManager.SendEmailAsync(user.Id, "Reset " + ConfigurationsHelper.ApplicationName + " Password", "Please reset your " + ConfigurationsHelper.ApplicationName + " password by clicking <a href=\"" + callbackUrl + "\">here</a>");
            }

            // Don't reveal that the user does not exist or is not confirmed for security measures.
            // Just give a success message in every case.

            JsonResult jResult = new JsonResult
            {
                Data = new { Success = true }
            };
            return jResult;
        }

        [AllowAnonymous]
        public ActionResult ResetPassword(string code, string userId)
        {
            ResetPasswordViewModel model = new ResetPasswordViewModel
            {
                Code = code,
                UserId = userId
            };

            return string.IsNullOrEmpty(code) || string.IsNullOrEmpty(userId) ? View("Error") : View(model);
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<JsonResult> ResetPassword(ResetPasswordViewModel model)
        {
            JsonResult jResult = new JsonResult();

            var user = await UserManager.FindByIdAsync(model.UserId);
            if (user != null)
            {
                var result = await UserManager.ResetPasswordAsync(user.Id, model.Code, model.Password);

                if (!result.Succeeded)
                {
                    jResult.Data = new { Success = result.Succeeded, Messages = string.Join("\n", result.Errors) };
                }
                else
                {
                    jResult.Data = new { Success = result.Succeeded, Messages = "Your password has been reset. Please login with your updated credentials now." };
                }
            }
            else
            {
                jResult.Data = new { Success = false, Messages = "Unable to reset password." };
            }

            return jResult;
        }


        public ActionResult UserProfile(string tab)
        {
            ProfileDetailsViewModel model = new ProfileDetailsViewModel
            {
                ActiveTab = tab,

                User = UserManager.FindById(User.Identity.GetUserId())
                ,
                States = UserProfileService.Instance.GetStateList(),
                userAddresses=UserProfileService.Instance.GetAddressList(User.Identity.GetUserId())
            };

            if (model.User == null) return HttpNotFound();

            if (Request.IsAjaxRequest())
            {
                return PartialView("_UserProfile", model);
            }
            else
            {
                return View(model);
            }
        }


        public ActionResult UserAddress(string tab)
        {
            ProfileAddressViewModel model = new ProfileAddressViewModel
            {
                ActiveTab = tab,

                User = UserManager.FindById(User.Identity.GetUserId())
                ,
                States = UserProfileService.Instance.GetStateList(),
                userAddresses = UserProfileService.Instance.GetAddressList(User.Identity.GetUserId())
            };

            if (model.User == null) return HttpNotFound();

            if (Request.IsAjaxRequest())
            {
                return PartialView("_UserAddresses", model);
            }
            else
            {
                return View(model);
            }
        }
        [HttpPost]
        public JsonResult Delete(int ID)
        {
            JsonResult result = new JsonResult();

            try
            {
                var operation = UserProfileService.Instance.Remove(ID);

                result.Data = new { Success = operation, Message = operation ? string.Empty : "Dashboard.Categories.Action.Validation.UnableToDeleteCategory".LocalizedString() };
            }
            catch (Exception ex)
            {
                result.Data = new { Success = false, Message = ex.Message };
            }

            return result;
        }

        [HttpPost]
        public Task<JsonResult> AddUserAddress(ProfileAddressViewModel model)
        {
            JsonResult jResult = new JsonResult();

            if (model != null && User.Identity.IsAuthenticated)
            {
                //var user = await UserManager.FindByIdAsync(User.Identity.GetUserId());
                UserAddress save_Address = new UserAddress()
                {
                    Address1 = model.Address,
                    City = model.City,
                    Zipcode = model.ZipCode,
                    StateId = model.StateCode,
                    Country = model.Country,
                    UserId = User.Identity.GetUserId()
                };


                var result =  UserProfileService.Instance.SaveAddress(save_Address);

                    jResult.Data = new { Success = result, Message = string.Join("\n", result) };

                    return Task.FromResult(jResult);
               
            }
            else
            {
                jResult.Data = new { Success = false, Message = "Invalid User" };
            }

            return Task.FromResult(jResult);
        }

      
        public ActionResult UpdateAddress(int? ID)
        {//first get the whole addresds into view then add models data into a form then edit its then cllcik on update button to send it through ajax method to update to datata base 
            AddressEditViewModel model = new AddressEditViewModel();
            model.States = UserProfileService.Instance.GetStateList();
            if (ID.HasValue)
            {
                var address = UserProfileService.Instance.GetAddressByID(ID.Value);

                if (address == null) return HttpNotFound();
                model.Id = address.Id;
                  model.Address= address.Address1;
                model.City = address.City;
                model.Country = address.Country;
                model.StateCode = address.StateId;
                model.ZipCode = address.Zipcode;


            }

            // model.Categories = CategoriesService.Instance.GetCategories();

            if (Request.IsAjaxRequest())
            {
                return PartialView("_UserAddressEdit", model);
            }
            else
            {
                return View(model);
            }
        }
        [HttpPost]
        public JsonResult UpdatesAddress(AddressEditViewModel model)
        {
            JsonResult jResult = new JsonResult();

            if (model != null && User.Identity.IsAuthenticated)
            {
                var update_address = UserProfileService.Instance.GetAddressByID(model.Id);

                if (update_address != null)
                {


                    update_address.Country = model.Country;
                    update_address.City = model.City;
                    update_address.Address1 = model.Address;
                    update_address.Zipcode = model.ZipCode;
                    update_address.StateId = model.StateCode;



                    var result = UserProfileService.Instance.Update(update_address);

                    jResult.Data = new { Success = result, Message = string.Join("\n", result) };

                    return jResult;
                }
            }
            else
            {
                jResult.Data = new { Success = false, Message = "Invalid User" };
            }

            return jResult;
        }
        [HttpPost]
        public async Task<JsonResult> UpdateProfile(UpdateProfileDetailsViewModel model)
        {
            JsonResult jResult = new JsonResult();

            if (model != null && User.Identity.IsAuthenticated)
            {
                var user = await UserManager.FindByIdAsync(User.Identity.GetUserId());

                if (user != null)
                {
                    user.FullName = model.FullName;
                    user.Email = model.Email;
                    user.UserName = model.Username;
                    user.PhoneNumber = model.PhoneNumber;
                    //  user.Country = model.Country;
                    //user.City = model.City;
                    //user.Address = model.Address;
                    // user.ZipCode = model.ZipCode;

                    user.Gstin = model.Gstin;
                    user.Gender = model.Gender;
                    user.Birthday = model.Birthday;
                    user.Anniversary = model.Anniversary;
                    user.SpouseBirthday = model.SpouseBirthday;
                    user.StateCode = model.StateCode;



                    var result = await UserManager.UpdateAsync(user);

                    jResult.Data = new { Success = result.Succeeded, Message = string.Join("\n", result.Errors) };

                    return jResult;
                }
            }
            else
            {
                jResult.Data = new { Success = false, Message = "Invalid User" };
            }

            return jResult;
        }

        public async Task<ActionResult> ChangePassword()
        {
            ProfileDetailsViewModel model = new ProfileDetailsViewModel
            {
                User = await UserManager.FindByIdAsync(User.Identity.GetUserId())
            };

            return PartialView("_ChangePassword", model);
        }

        public async Task<JsonResult> UpdatePassword(UpdatePasswordViewModel model)
        {
            JsonResult jResult = new JsonResult();

            if (model != null && User.Identity.IsAuthenticated)
            {
                var user = await UserManager.FindByIdAsync(User.Identity.GetUserId());

                if (user != null)
                {
                    var result = await UserManager.ChangePasswordAsync(User.Identity.GetUserId(), model.OldPassword, model.NewPassword);

                    jResult.Data = new { Success = result.Succeeded, Message = string.Join("\n", result.Errors) };

                    await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
                }
            }
            else
            {
                jResult.Data = new { Success = false, Message = "Invalid User" };
            }

            return jResult;
        }

        public async Task<ActionResult> ChangeAvatar()
        {
            ProfileDetailsViewModel model = new ProfileDetailsViewModel
            {
                User = await UserManager.FindByIdAsync(User.Identity.GetUserId())
            };

            return PartialView("_ChangeAvatar", model);
        }

        public async Task<JsonResult> UpdateAvatar(int pictureID)
        {
            JsonResult jResult = new JsonResult();

            if (pictureID > 0 && User.Identity.IsAuthenticated)
            {
                var user = await UserManager.FindByIdAsync(User.Identity.GetUserId());

                if (user != null)
                {
                    user.PictureID = pictureID;

                    var result = await UserManager.UpdateAsync(user);
                    await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);

                    jResult.Data = new { Success = result.Succeeded, Message = string.Join("\n", result.Errors) };
                }
            }
            else
            {
                jResult.Data = new { Success = false, Message = "Invalid User" };
            }

            return jResult;
        }

        // Used for XSRF protection when adding external logins
        private const string XsrfKey = "XsrfId";
        internal class ChallengeResult : HttpUnauthorizedResult
        {
            public ChallengeResult(string provider, string redirectUri)
                : this(provider, redirectUri, null)
            {
            }

            public ChallengeResult(string provider, string redirectUri, string userId)
            {
                LoginProvider = provider;
                RedirectUri = redirectUri;
                UserId = userId;
            }

            public string LoginProvider { get; set; }
            public string RedirectUri { get; set; }
            public string UserId { get; set; }

            public override void ExecuteResult(ControllerContext context)
            {
                var properties = new AuthenticationProperties { RedirectUri = RedirectUri };
                if (UserId != null)
                {
                    properties.Dictionary[XsrfKey] = UserId;
                }
                context.HttpContext.GetOwinContext().Authentication.Challenge(properties, LoginProvider);
            }
        }
    }
}