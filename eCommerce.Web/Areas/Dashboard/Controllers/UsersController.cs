using eCommerce.Services;
using eCommerce.Shared.Enums;
using eCommerce.Shared.Helpers;
using eCommerce.Web.Areas.Dashboard.ViewModels;
using eCommerce.Web.ViewModels;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace eCommerce.Web.Areas.Dashboard.Controllers
{
    public class UsersController : DashboardBaseController
    {
        private eCommerceUserManager _userManager;
        private eCommerceRoleManager _roleManager;
        private eCommerceSignInManager _signInManager;

        public UsersController()
        {
        }

        public UsersController(eCommerceUserManager userManager, eCommerceRoleManager roleManager, eCommerceSignInManager signInManager)
        {
            UserManager = userManager;
            RoleManager = roleManager;
            SignInManager = signInManager;
        }

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

        public ActionResult Index(string roleID, string searchTerm, int? pageNo)
        {
            var pageSize = (int)RecordSizeEnums.Size10;

            UsersListingViewModel model = new UsersListingViewModel
            {
                RoleID = roleID,
                SearchTerm = searchTerm,

                Roles = RoleManager.Roles.ToList()
            };

            var users = UserManager.Users;

            if (!string.IsNullOrEmpty(roleID))
            {
                users = users.Where(x => x.Roles.FirstOrDefault(y => y.RoleId == roleID) != null);
            }

            if (!string.IsNullOrEmpty(searchTerm))
            {
                users = users.Where(x => x.Email.ToLower().Contains(searchTerm.ToLower()) || x.UserName.ToLower().Contains(searchTerm.ToLower()));
            }

            pageNo = pageNo ?? 1;
            var skipCount = (pageNo.Value - 1) * pageSize;

            model.Users = users.OrderByDescending(x => x.RegisteredOn).Skip(skipCount).Take(pageSize).ToList();

            model.Pager = new Pager(users.Count(), pageNo, pageSize);

            return View(model);
        }

        public async Task<ActionResult> UserDetails(string userID, bool isPartial = false)
        {
            UserDetailsViewModel model = new UserDetailsViewModel();
            var useraddress = UserProfileService.Instance.GetAddressList(userID);
           var defaultaddress= useraddress.FirstOrDefault();
            var user = await UserManager.FindByIdAsync(userID);

            if (user != null)
            {
                model.User = user;
                model.Address = defaultaddress.Address1;
                model.City = defaultaddress.City;
                model.Country = defaultaddress.Country;
                model.ZipCode = defaultaddress.Zipcode.ToString();
            
            }

            if (isPartial || Request.IsAjaxRequest())
            {
                return PartialView("_UserDetails", model);
            }
            else
            {
                return View(model);
            }
        }

        [HttpPost]
        public async Task<JsonResult> Action(UserDetailsViewModel model)
        {
            JsonResult json = new JsonResult();

            try
            {
                if (model != null)
                {
                    var user = await UserManager.FindByIdAsync(model.ID);

                    if (user != null)
                    {
                        user.FullName = model.FullName;
                        //user.Country = model.Country;
                        //user.City = model.City;
                        //user.Address = model.Address;
                        //user.ZipCode = model.ZipCode; we have separted the address fields into useraddress table instead of users table thats why this field are commented //01.09.2021 @asad

                        var result = await UserManager.UpdateAsync(user);

                        if (result.Succeeded)
                        {
                            json.Data = new { Success = result.Succeeded, Message = "Dashboard.UserDetails.Info.Action.Validation.InfoUpdated".LocalizedString() };
                        }
                        else
                        {
                            throw new Exception("Dashboard.UserDetails.Info.Action.Validation.UnabletoUpdateInfo".LocalizedString());
                        }
                    }
                    else
                    {
                        throw new Exception("Dashboard.UserDetails.Info.Action.Validation.UserNotFound".LocalizedString());
                    }
                }
                else
                {
                    throw new Exception("Dashboard.UserDetails.Info.Action.Validation.DataNotFormatted".LocalizedString());
                }
            }
            catch (Exception ex)
            {
                json.Data = new { Success = false, Message = ex.Message };
            }

            return json;
        }

        [HttpPost]
        public async Task<JsonResult> Delete(string userID)
        {
            JsonResult json = new JsonResult();

            try
            {
                var user = await UserManager.FindByIdAsync(userID);

                if (user != null)
                {
                    var result = await UserManager.DeleteAsync(user);

                    if (result.Succeeded)
                    {
                        json.Data = new { Success = result.Succeeded, Message = "Dashboard.UserDetails.Info.Action.Validation.UserDeleted".LocalizedString() };
                    }
                    else
                    {
                        throw new Exception("Dashboard.UserDetails.Info.Action.Validation.UnabletoDeleteUser".LocalizedString());
                    }
                }
                else
                {
                    throw new Exception("Dashboard.UserDetails.Info.Action.Validation.UserNotFound".LocalizedString());
                }
            }
            catch (Exception ex)
            {
                json.Data = new { Success = false, Message = ex.Message };
            }

            return json;
        }
    }
}