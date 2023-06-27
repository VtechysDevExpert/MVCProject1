using eCommerce.Entities;
using eCommerce.Entities.CustomEntities;
using eCommerce.Services;
using eCommerce.Shared.Helpers;
using eCommerce.Web.ViewModels;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using System;
using System.Linq;
using System.Web;
using System.Web.Mvc;
namespace eCommerce.Web.Controllers
{
    public class WishListController : PublicBaseController
    {
        private eCommerceSignInManager _signInManager;
        private eCommerceUserManager _userManager;
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
        public WishListController()
        {
        }
        public WishListController(eCommerceUserManager userManager, eCommerceSignInManager signInManager)
        {
            UserManager = userManager;
            SignInManager = signInManager;
        }
        public JsonResult AddToWishList(int itemID, int quantity = 1)
        {
            JsonResult json = new JsonResult();
            var message = string.Empty;
            var product = ProductsService.Instance.GetProductByID(itemID);
            var wish = WishListService.Instance.GetWishListByID(User.Identity.GetUserId(), itemID);

            if (product != null)
            {
                if (User.Identity.IsAuthenticated)
                {
                    //cheking if item alredy exist or not!!
                    if (wish.Count > 0)
                    {
                        message = "Item already Exist in the wishlist";
                        json.Data = new { Success = false, Message = message };
                    }
                    else
                    {
                        WishList wishlistitem = new WishList
                        {
                            ItemID = product.ID,
                            UserID = User.Identity.GetUserId()
                        };
                        WishListService.Instance.Save(wishlistitem);
                        message = "Item added in the WishList!";
                        json.Data = new { Success = true, Message = message };
                    }


                }
                else
                {
                    message = "Please log in to add the product to your wishlist.";
                    json.Data = new { Success = false, Message = message };
                }
            }
            else
            {
                json.Data = new { Success = false, Message = "PP.Shopping.ItemNotFound".LocalizedString() };
            }
            return json;
        }
        public ActionResult WishList()
        {
            WishListViewModels model = new WishListViewModels();
            if (User.Identity.IsAuthenticated)
            {
                model.WishLists = WishListService.Instance.GetWishListByIDs(User.Identity.GetUserId());
                model.ProductIDs = model.WishLists.Select(x => x.ItemID).ToList();
                model.ProductDetails_vm = model.ProductDetails_vm = ProductDetailsService.Instance.GetProductDetails();
                if (model.ProductIDs.Count > 0)
                {
                    model.Products = ProductsService.Instance.GetProductsByIDs(model.ProductIDs);
                }
            }
            if (Request.IsAjaxRequest())
            {
                return PartialView("_WishListItems", model);
            }

            return View("WishList", model);
        }
        [HttpPost]
        public JsonResult Delete(int ID)
        {
            JsonResult result = new JsonResult();
            try
            {
                var operation = WishListService.Instance.Remove(ID);
                result.Data = new { Success = operation, Message = operation ? string.Empty : "Dashboard.Categories.Action.Validation.UnableToDeleteCategory".LocalizedString() };
            }
            catch (Exception ex)
            {
                result.Data = new { Success = false, Message = ex.Message };
            }
            return result;
        }
    }
}