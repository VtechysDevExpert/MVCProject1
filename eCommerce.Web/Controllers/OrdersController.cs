using AuthorizeNet.Api.Contracts.V1;
using eCommerce.Entities;
using eCommerce.Services;
using eCommerce.Shared.Enums;
using eCommerce.Shared.Helpers;
using eCommerce.Web.ViewModels;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace eCommerce.Web.Controllers
{
    public class OrdersController : PublicBaseController
    {
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

        public OrdersController()
        {
        }

        public OrdersController(eCommerceUserManager userManager, eCommerceSignInManager signInManager, eCommerceRoleManager roleManager)
        {
            UserManager = userManager;
            SignInManager = signInManager;
            RoleManager = roleManager;
        }

        public async Task<JsonResult> PlaceOrder(PlaceOrderCrediCardViewModel model)
        {
            JsonResult jsonResult = new JsonResult();
           
        var errorDetails = string.Empty;

            if (model != null && ModelState.IsValid)
            {
                if (SessionHelper.CartItems != null && SessionHelper.CartItems.Count > 0)
                {
                    model.ProductIDs = SessionHelper.CartItems.Select(x => x.ID).ToList();

                    if (model.ProductIDs.Count > 0)
                    {
                        model.productDetails = ProductDetailsService.Instance.GetProductsByIDs(model.ProductIDs);

                        model.Products = ProductsService.Instance.GetProductsByIDs(model.productDetails.Select(X => X.ProductID).ToList());
                      //  model.Products = ProductsService.Instance.GetProductsByIDs(model.ProductIDs);
                    }
                }

                if (SessionHelper.CartItems != null && SessionHelper.CartItems.Count > 0 && model.Products != null && model.Products.Count > 0)
                {
                    var newOrder = new Order();

                    if (User.Identity.IsAuthenticated)
                    {
                        newOrder.CustomerID = User.Identity.GetUserId();
                        var UsersDetails = UserProfileService.Instance.GetUser(User.Identity.GetUserId());
                        if (UsersDetails != null)

                        {
                            newOrder.UserId = UsersDetails.UserId;
                            newOrder.StateId = UsersDetails.StateCode;
                        }
                    }
                    else if (model.CreateAccount)
                    {
                        try
                        {
                            var user = new eCommerceUser { FullName = model.FullName, UserName = model.Email, Email = model.Email, PhoneNumber = model.PhoneNumber, RegisteredOn = DateTime.Now };
                            var result = await UserManager.CreateAsync(user);

                            if (result.Succeeded)
                            {
                                if (await RoleManager.RoleExistsAsync("User"))
                                {
                                    //assign User role to newly registered user
                                    await UserManager.AddToRoleAsync(user.Id, "User");
                                }

                                await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);

                                //send account register notification email
                                await UserManager.SendEmailAsync(user.Id, EmailTextHelpers.AccountRegisterEmailSubject(AppDataHelper.CurrentLanguage.ID), EmailTextHelpers.AccountRegisterEmailBody(AppDataHelper.CurrentLanguage.ID, Url.Action("Login", "Users", null, protocol: Request.Url.Scheme)));

                                newOrder.CustomerID = user.Id;
                            }
                            else
                            {
                                jsonResult.Data = new
                                {
                                    Success = false,
                                    Message = string.Join("<br />", result.Errors)
                                };
                                return jsonResult;
                            }
                        }
                        catch
                        {
                            jsonResult.Data = new
                            {
                                Success = false,
                                Message = string.Format("An error occured while registering user.")
                            };
                            return jsonResult;
                        }
                    }
                    else
                    {
                        newOrder.IsGuestOrder = true;
                    }

                    newOrder.CustomerName = model.FullName;
                    newOrder.CustomerEmail = model.Email;
                    newOrder.CustomerPhone = model.PhoneNumber;
                    newOrder.CustomerCountry = model.Country;
                    newOrder.CustomerCity = model.City;
                    newOrder.CustomerAddress = model.Address;
                    newOrder.CustomerZipCode = model.ZipCode;
                    //making payment false; becoz it will be updated to true once user make payment
                    newOrder.IsPaid = false;
                    newOrder.OrderItems = new List<OrderItem>();

                    foreach (var carts in SessionHelper.CartItems)
                    {
                        var product = model.Products.FirstOrDefault(x => x.ID == carts.ItemID);
                        var currentLanguageProductRecord = product.ProductRecords.FirstOrDefault(x => x.LanguageID == AppDataHelper.CurrentLanguage.ID);
                        var productDetails = model.productDetails.FirstOrDefault(x => x.ID == carts.ID);


                        var orderItem = new OrderItem
                        {

                            ProductID = product.ID,
                            ProductName = currentLanguageProductRecord != null ? currentLanguageProductRecord.Name : string.Format("Product ID : {0}", product.ID),

                            ItemPrice = productDetails.Discount.HasValue && productDetails.Discount.Value > 0 ? productDetails.Discount.Value : productDetails.Amount,

                            Quantity = SessionHelper.CartItems.FirstOrDefault(x => x.ItemID == productDetails.ProductID && x.Weight == productDetails.Weight).Quantity,
                            Weight = SessionHelper.CartItems.FirstOrDefault(x => x.ItemID == productDetails.ProductID && x.Weight == productDetails.Weight).Weight
                        };


                        newOrder.OrderItems.Add(orderItem);
                    }


                    //foreach (var product in SessionHelper.CartItems.Select(x => model.Products.FirstOrDefault(y => y.ID == x.ItemID)))
                    //{
                    //    var currentLanguageProductRecord = product.ProductRecords.FirstOrDefault(x => x.LanguageID == AppDataHelper.CurrentLanguage.ID);

                    //    var orderItem = new OrderItem
                    //    {
                    //        ProductID = product.ID,
                    //        ProductName = currentLanguageProductRecord != null ? currentLanguageProductRecord.Name : string.Format("Product ID : {0}", product.ID),
                    //        ItemPrice = product.Price,
                    //        Quantity = SessionHelper.CartItems.FirstOrDefault(x => x.ItemID == product.ID).Quantity
                    //    };

                    //    newOrder.OrderItems.Add(orderItem);
                    //}

                    newOrder.OrderCode = Guid.NewGuid().ToString();
                    newOrder.TotalAmmount = newOrder.OrderItems.Sum(x => (x.ItemPrice * x.Quantity));
                    newOrder.DeliveryCharges = ConfigurationsHelper.FlatDeliveryCharges;
                    // fixed fncd for now... make sure you change it on multiple places on this file because of multiple payments methods.
                    newOrder.Fncd = 1;
                  
                    //Applying Promo/voucher
                    if (!string.IsNullOrEmpty(SessionHelper.PromoCode))
                    {
                        var promo = SessionHelper.Promo;
                        if (promo != null && promo.Value > 0)
                        {
                            if (promo.ValidTill == null || promo.ValidTill >= DateTime.Now)
                            {
                                newOrder.PromoID = promo.ID;

                                if (promo.PromoType == (int)PromoTypes.Percentage)
                                {
                                    newOrder.Discount = Math.Round((newOrder.TotalAmmount * promo.Value) / 100);
                                }
                                else if (promo.PromoType == (int)PromoTypes.Amount)
                                {
                                    newOrder.Discount = promo.Value;
                                }
                            }
                        }
                    }

                    newOrder.FinalAmmount = newOrder.TotalAmmount + newOrder.DeliveryCharges - newOrder.Discount;
                    newOrder.PaymentMethod = (int)PaymentMethods.CreditCard;

                    newOrder.OrderHistory = new List<OrderHistory>() {
                        new OrderHistory() {
                            OrderStatus = (int)OrderStatus.Placed,
                            ModifiedOn = DateTime.Now,
                            Note = "Order Placed."
                        }
                    };

                    newOrder.PlacedOn = DateTime.Now;

                    #region ExecuteAuthorizeNetPayment Execution

                    var creditCardInfo = new creditCardType
                    {
                        cardNumber = model.CCCardNumber,
                        expirationDate = string.Format("{0}{1}", model.CCExpiryMonth, model.CCExpiryYear),
                        cardCode = model.CCCVC
                    };

                    var authorizeNetResponse = AuthorizeNetHelper.ExecutePayment(newOrder, creditCardInfo);

                    #endregion

                    if (authorizeNetResponse.Success)
                    {
                        newOrder.TransactionID = authorizeNetResponse.Response.transactionResponse.transId;

                        if (OrdersService.Instance.SaveOrder(newOrder))
                        {
                            SessionHelper.ClearCart();

                            ProductsService.Instance.UpdateProductQuantities(newOrder);

                            if (!newOrder.IsGuestOrder)
                            {
                                //send order placed notification email
                                await UserManager.SendEmailAsync(newOrder.CustomerID, EmailTextHelpers.OrderPlacedEmailSubject(AppDataHelper.CurrentLanguage.ID, newOrder.ID), EmailTextHelpers.OrderPlacedEmailBody(AppDataHelper.CurrentLanguage.ID, newOrder.ID, Url.Action("Tracking", "Orders", new { orderID = newOrder.ID }, protocol: Request.Url.Scheme)));

                                //send order placed notification email to admin emails
                                await new EmailService().SendToEmailAsync(ConfigurationsHelper.SendGrid_FromEmailAddressName, ConfigurationsHelper.SendGrid_FromEmailAddress, ConfigurationsHelper.AdminEmailAddress, EmailTextHelpers.OrderPlacedEmailSubject_Admin(AppDataHelper.CurrentLanguage.ID, newOrder.ID), EmailTextHelpers.OrderPlacedEmailBody_Admin(AppDataHelper.CurrentLanguage.ID, newOrder.ID, Url.Action("Details", "Orders", new { area = "Dashboard", ID = newOrder.ID }, protocol: Request.Url.Scheme)));
                            }

                            jsonResult.Data = new
                            {
                                Success = true,
                                OrderID = newOrder.ID
                            };
                        }
                        else
                        {
                            jsonResult.Data = new
                            {
                                Success = false,
                                Message = "System can not take any order."
                            };
                        }
                    }
                    else
                    {
                        jsonResult.Data = new
                        {
                            Success = authorizeNetResponse.Success,
                            Message = authorizeNetResponse.Message
                        };
                    }
                }
                else
                {
                    jsonResult.Data = new
                    {
                        Success = false,
                        Message = "Invalid products in cart."
                    };
                }
            }
            else
            {
                jsonResult.Data = new
                {
                    Success = false,
                    Message = string.Join("\n", ModelState.Values
                                        .SelectMany(x => x.Errors)
                                        .Select(x => x.ErrorMessage))
                };
            }

            return jsonResult;
        }

        public async Task<JsonResult> PlaceOrderViaCashOnDelivery(PlaceOrderCashOnDeliveryViewModel model)
        {
            JsonResult jsonResult = new JsonResult();
            model.Financial_Year = 1;
            var errorDetails = string.Empty;
            if (User.Identity.IsAuthenticated)
            {

            
            if (model != null && ModelState.IsValid)
            {
                if (SessionHelper.CartItems != null && SessionHelper.CartItems.Count > 0)
                {
                    model.ProductIDs = SessionHelper.CartItems.Select(x => x.ID).ToList();

                    if (model.ProductIDs.Count > 0)
                    {
                       
                        model.productDetails = ProductDetailsService.Instance.GetProductsByIDs(model.ProductIDs);

                        model.Products = ProductsService.Instance.GetProductsByIDs(model.productDetails.Select(X => X.ProductID).ToList());
                    }
                }

                if (SessionHelper.CartItems != null && SessionHelper.CartItems.Count > 0 && model.Products != null && model.Products.Count > 0)
                {
                    var newOrder = new Order();

                    if (User.Identity.IsAuthenticated)
                    {
                        newOrder.CustomerID = User.Identity.GetUserId();
                        var UsersDetails = UserProfileService.Instance.GetUser(User.Identity.GetUserId());
                        if (UsersDetails!=null)

                        {
                            newOrder.UserId = UsersDetails.UserId;
                            newOrder.StateId = UsersDetails.StateCode;
                        }
                    }
                    else if (model.CreateAccount)
                    {
                        try
                        {
                            var user = new eCommerceUser { FullName = model.FullName, UserName = model.Email, Email = model.Email, PhoneNumber = model.PhoneNumber, RegisteredOn = DateTime.Now };

                            var result = await UserManager.CreateAsync(user);

                            if (result.Succeeded)
                            {
                                if (await RoleManager.RoleExistsAsync("User"))
                                {
                                    //assign User role to newly registered user
                                    await UserManager.AddToRoleAsync(user.Id, "User");
                                }

                                await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);

                                //send account register notification email
                                await UserManager.SendEmailAsync(user.Id, EmailTextHelpers.AccountRegisterEmailSubject(AppDataHelper.CurrentLanguage.ID), EmailTextHelpers.AccountRegisterEmailBody(AppDataHelper.CurrentLanguage.ID, Url.Action("Login", "Users", null, protocol: Request.Url.Scheme)));

                                newOrder.CustomerID = user.Id;
                            }
                            else
                            {
                                jsonResult.Data = new
                                {
                                    Success = false,
                                    Message = string.Join("<br />", result.Errors)
                                };
                                return jsonResult;
                            }
                        }
                        catch
                        {
                            jsonResult.Data = new
                            {
                                Success = false,
                                Message = string.Format("An error occured while registering user.")
                            };
                            return jsonResult;
                        }
                    }
                    else
                        {
                            newOrder.IsGuestOrder = true;
                        newOrder.StateId = 21;

                    }
                        
                            newOrder.CustomerName = model.FullName;
                            newOrder.CustomerEmail = model.Email;
                            newOrder.CustomerPhone = model.PhoneNumber;
                            newOrder.CustomerCountry = model.Country;
                            newOrder.CustomerCity = model.City;
                            newOrder.CustomerAddress = model.Address;
                            newOrder.CustomerZipCode = model.ZipCode;

                        
                   

                    newOrder.OrderItems = new List<OrderItem>();
                  
                       

                    foreach (var carts in SessionHelper.CartItems)
                    {
                        var product = model.Products.FirstOrDefault(x => x.ID == carts.ItemID);
                        var currentLanguageProductRecord = product.ProductRecords.FirstOrDefault(x => x.LanguageID == AppDataHelper.CurrentLanguage.ID);
                        var productDetails = model.productDetails.FirstOrDefault(x => x.ID == carts.ID );


                            var orderItem = new OrderItem
                            {
                            Fncd = model.Financial_Year,
                            ProductID = product.ID,
                            ProductName = currentLanguageProductRecord != null ? currentLanguageProductRecord.Name : string.Format("Product ID : {0}", product.ID),

                                //ItemPrice = productDetails.Discount.HasValue && productDetails.Discount.Value > 0 ? productDetails.Discount.Value : productDetails.Amount,
                                ItemPrice =productDetails.Amount,

                                Quantity = SessionHelper.CartItems.FirstOrDefault(x => x.ItemID == productDetails.ProductID && x.Weight== productDetails.Weight).Quantity,
                            Weight= SessionHelper.CartItems.FirstOrDefault(x => x.ItemID == productDetails.ProductID && x.Weight == productDetails.Weight).Weight
                        };
                        newOrder.OrderItems.Add(orderItem);
                    }

                    newOrder.OrderCode = Guid.NewGuid().ToString();
                    newOrder.TotalAmmount = newOrder.OrderItems.Sum(x => (x.ItemPrice * x.Quantity));
                    newOrder.DeliveryCharges = ConfigurationsHelper.FlatDeliveryCharges;
                   
                    // fixed fncd for now... make sure you change it on multiple places on this file because of multiple payments methods.
                    newOrder.Fncd = model.Financial_Year;

                   
                    //Applying Promo/voucher 
                    if (!string.IsNullOrEmpty(SessionHelper.PromoCode))
                    {
                        var promo = SessionHelper.Promo;
                        if (promo != null && promo.Value > 0)
                        {
                            if (promo.ValidTill == null || promo.ValidTill >= DateTime.Now)
                            {
                                newOrder.PromoID = promo.ID;

                                if (promo.PromoType == (int)PromoTypes.Percentage)
                                {
                                    newOrder.Discount = Math.Round((newOrder.TotalAmmount * promo.Value) / 100);
                                }
                                else if (promo.PromoType == (int)PromoTypes.Amount)
                                {
                                    newOrder.Discount = promo.Value;
                                }
                            }
                        }
                    }

                    newOrder.FinalAmmount = newOrder.TotalAmmount + newOrder.DeliveryCharges - newOrder.Discount;
                    newOrder.PaymentMethod = (int)PaymentMethods.NetBanking;
                        newOrder.OrderHistory = new List<OrderHistory>();
                        foreach (var Prod_id in SessionHelper.CartItems)
                        {
                            var OrderHistory = new OrderHistory
                             {

                                OrderStatus = (int)OrderStatus.Placed,
                                      ModifiedOn = DateTime.Now,Fncd= model.Financial_Year,
                                      Note = "Order Placed.",
                                      Weight=Prod_id.Weight,
                                     ProductID=Prod_id.ItemID,

                            };

                            newOrder.OrderHistory.Add(OrderHistory);

                        }
                    

                    newOrder.PlacedOn = DateTime.Now;

                    if (OrdersService.Instance.SaveOrder(newOrder))
                    {
                        
                        
                        
                        //SessionHelper.ClearCart();
                        ProductsService.Instance.UpdateProductQuantities(newOrder);
                            if (!newOrder.IsGuestOrder)
                            {
                                //send order placed notification email


                                //var mailid = string.Empty;
                                //var message = string.Empty;
                                //var UserName = string.Empty;
                                //var PhoneNumber = string.Empty;
                                var _ProductName = string.Empty;
                                var UsersDetails = UserProfileService.Instance.GetUser(User.Identity.GetUserId());
                                if (UsersDetails != null)

                                {
                                    string Msg = string.Empty;
                                    Msg = "Your order is Placed." ;
                                    NotificationHelper.SendSms(UsersDetails.PhoneNumber, Msg, _ProductName);
                                    NotificationHelper.SendMail(UsersDetails.Email, UsersDetails.FullName, Msg);
                                }


                                




                                //    await UserManager.SendEmailAsync(newOrder.CustomerID, EmailTextHelpers.OrderPlacedEmailSubject(AppDataHelper.CurrentLanguage.ID, newOrder.ID), EmailTextHelpers.OrderPlacedEmailBody(AppDataHelper.CurrentLanguage.ID, newOrder.ID, Url.Action("Tracking", "Orders", new { orderID = newOrder.ID }, protocol: Request.Url.Scheme)));

                                //    //send order placed notification email to admin emails
                                //    await new EmailService().SendToEmailAsync(ConfigurationsHelper.SendGrid_FromEmailAddressName, ConfigurationsHelper.SendGrid_FromEmailAddress, ConfigurationsHelper.AdminEmailAddress, EmailTextHelpers.OrderPlacedEmailSubject_Admin(AppDataHelper.CurrentLanguage.ID, newOrder.ID), EmailTextHelpers.OrderPlacedEmailBody_Admin(AppDataHelper.CurrentLanguage.ID, newOrder.ID, Url.Action("Details", "Orders", new { area = "Dashboard", ID = newOrder.ID }, protocol: Request.Url.Scheme)));
                                //}
                            }
                                jsonResult.Data = new
                        {
                            Success = true,
                            OrderID = newOrder.ID
                        };
                    }
                    else
                    {
                        jsonResult.Data = new
                        {
                            Success = false,
                            Message = "System can not take any order."
                        };
                    }
                }
                else
                {
                    jsonResult.Data = new
                    {
                        Success = false,
                        Message = "Invalid products in cart."
                    };
                }
            }
            else
            {
                jsonResult.Data = new
                {
                    Success = false,
                    Message = string.Join("\n", ModelState.Values
                                        .SelectMany(x => x.Errors)
                                        .Select(x => x.ErrorMessage))
                };
            }
            }
            else
            {
                jsonResult.Data = new
                {
                    Success = false,
                    Message = "Please Login First!"
                };
            }
            return jsonResult;
        }
          
        public async Task<JsonResult> PlaceOrderViaPayPal(PlaceOrderPayPalViewModel model)
        {
            JsonResult jsonResult = new JsonResult();

            var errorDetails = string.Empty;

            if (model != null && ModelState.IsValid)
            {
                if (SessionHelper.CartItems != null && SessionHelper.CartItems.Count > 0)
                {
                    model.ProductIDs = SessionHelper.CartItems.Select(x => x.ID).ToList();

                    if (model.ProductIDs.Count > 0)
                    {
                        model.productDetails = ProductDetailsService.Instance.GetProductsByIDs(model.ProductIDs);
                        model.Products = ProductsService.Instance.GetProductsByIDs(model.productDetails.Select(X => X.ProductID).ToList());
                    }
                }

                if (SessionHelper.CartItems != null && SessionHelper.CartItems.Count > 0 && model.Products != null && model.Products.Count > 0)
                {
                    var newOrder = new Order();

                    if (User.Identity.IsAuthenticated)
                    {
                        newOrder.CustomerID = User.Identity.GetUserId();
                        var UsersDetails = UserProfileService.Instance.GetUser(User.Identity.GetUserId());
                        if (UsersDetails != null)

                        {
                            newOrder.UserId = UsersDetails.UserId;
                            newOrder.StateId = UsersDetails.StateCode;
                        }
                    }
                    else if (model.CreateAccount)
                    {
                        try
                        {
                            var user = new eCommerceUser { FullName = model.FullName, UserName = model.Email, Email = model.Email, PhoneNumber = model.PhoneNumber, RegisteredOn = DateTime.Now };

                            var result = await UserManager.CreateAsync(user);

                            if (result.Succeeded)
                            {
                                if (await RoleManager.RoleExistsAsync("User"))
                                {
                                    //assign User role to newly registered user
                                    await UserManager.AddToRoleAsync(user.Id, "User");
                                }

                                await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);

                                //send account register notification email
                                await UserManager.SendEmailAsync(user.Id, EmailTextHelpers.AccountRegisterEmailSubject(AppDataHelper.CurrentLanguage.ID), EmailTextHelpers.AccountRegisterEmailBody(AppDataHelper.CurrentLanguage.ID, Url.Action("Login", "Users", null, protocol: Request.Url.Scheme)));

                                newOrder.CustomerID = user.Id;
                            }
                            else
                            {
                                jsonResult.Data = new
                                {
                                    Success = false,
                                    Message = string.Join("<br />", result.Errors)
                                };
                                return jsonResult;
                            }
                        }
                        catch
                        {
                            jsonResult.Data = new
                            {
                                Success = false,
                                Message = string.Format("An error occured while registering user.")
                            };
                            return jsonResult;
                        }
                    }
                    else
                    {
                        newOrder.IsGuestOrder = true;
                    }

                    newOrder.CustomerName = model.FullName;
                    newOrder.CustomerEmail = model.Email;
                    newOrder.CustomerPhone = model.PhoneNumber;
                    newOrder.CustomerCountry = model.Country;
                    newOrder.CustomerCity = model.City;
                    newOrder.CustomerAddress = model.Address;
                    newOrder.CustomerZipCode = model.ZipCode;

                    newOrder.OrderItems = new List<OrderItem>();

                    foreach (var carts in SessionHelper.CartItems)
                    {
                        var product = model.Products.FirstOrDefault(x => x.ID == carts.ItemID);
                        var currentLanguageProductRecord = product.ProductRecords.FirstOrDefault(x => x.LanguageID == AppDataHelper.CurrentLanguage.ID);
                        var productDetails = model.productDetails.FirstOrDefault(x => x.ID == carts.ID);


                        var orderItem = new OrderItem
                        {

                            ProductID = product.ID,
                            ProductName = currentLanguageProductRecord != null ? currentLanguageProductRecord.Name : string.Format("Product ID : {0}", product.ID),

                            ItemPrice = productDetails.Discount.HasValue && productDetails.Discount.Value > 0 ? productDetails.Discount.Value : productDetails.Amount,

                            Quantity = SessionHelper.CartItems.FirstOrDefault(x => x.ItemID == productDetails.ProductID && x.Weight == productDetails.Weight).Quantity,
                            Weight = SessionHelper.CartItems.FirstOrDefault(x => x.ItemID == productDetails.ProductID && x.Weight == productDetails.Weight).Weight
                        };


                        newOrder.OrderItems.Add(orderItem);
                    }


                    //foreach (var product in SessionHelper.CartItems.Select(x => model.Products.FirstOrDefault(y => y.ID == x.ItemID)))
                    //{
                    //    var currentLanguageProductRecord = product.ProductRecords.FirstOrDefault(x => x.LanguageID == AppDataHelper.CurrentLanguage.ID);

                    //    var orderItem = new OrderItem
                    //    {
                    //        ProductID = product.ID,
                    //        ProductName = currentLanguageProductRecord != null ? currentLanguageProductRecord.Name : string.Format("Product ID : {0}", product.ID),
                    //        ItemPrice = product.Price,
                    //        Quantity = SessionHelper.CartItems.FirstOrDefault(x => x.ItemID == product.ID).Quantity
                    //    };

                    //    newOrder.OrderItems.Add(orderItem);
                    //}

                    newOrder.OrderCode = Guid.NewGuid().ToString();
                    newOrder.TotalAmmount = newOrder.OrderItems.Sum(x => (x.ItemPrice * x.Quantity));
                    newOrder.DeliveryCharges = ConfigurationsHelper.FlatDeliveryCharges;
                    // fixed fncd for now... make sure you change it on multiple places on this file because of multiple payments methods.
                    newOrder.Fncd = 1;
                    //Applying Promo/voucher 
                    if (!string.IsNullOrEmpty(SessionHelper.PromoCode))
                    {
                        var promo = SessionHelper.Promo;
                        if (promo != null && promo.Value > 0)
                        {
                            if (promo.ValidTill == null || promo.ValidTill >= DateTime.Now)
                            {
                                newOrder.PromoID = promo.ID;

                                if (promo.PromoType == (int)PromoTypes.Percentage)
                                {
                                    newOrder.Discount = Math.Round((newOrder.TotalAmmount * promo.Value) / 100);
                                }
                                else if (promo.PromoType == (int)PromoTypes.Amount)
                                {
                                    newOrder.Discount = promo.Value;
                                }
                            }
                        }
                    }

                    newOrder.FinalAmmount = newOrder.TotalAmmount + newOrder.DeliveryCharges - newOrder.Discount;
                    newOrder.PaymentMethod = (int)PaymentMethods.RazorPay;
                    newOrder.TransactionID = model.TransactionID;

                    newOrder.OrderHistory = new List<OrderHistory>() {
                        new OrderHistory() {
                            OrderStatus = (int)OrderStatus.Placed,
                            ModifiedOn = DateTime.Now,
                            Note = string.Format("Order placed via PayPal by PayPal Account Name: {0} ({1})", model.AccountName, model.AccountEmail)
                        }
                    };

                    newOrder.PlacedOn = DateTime.Now;

                    if (OrdersService.Instance.SaveOrder(newOrder))
                    {
                        SessionHelper.ClearCart();

                        ProductsService.Instance.UpdateProductQuantities(newOrder);

                        if (!newOrder.IsGuestOrder)
                        {
                            
                            //send order placed notification email
                            await UserManager.SendEmailAsync(newOrder.CustomerID, EmailTextHelpers.OrderPlacedEmailSubject(AppDataHelper.CurrentLanguage.ID, newOrder.ID), EmailTextHelpers.OrderPlacedEmailBody(AppDataHelper.CurrentLanguage.ID, newOrder.ID, Url.Action("Tracking", "Orders", new { orderID = newOrder.ID }, protocol: Request.Url.Scheme)));

                            //send order placed notification email to admin emails
                            await new EmailService().SendToEmailAsync(ConfigurationsHelper.SendGrid_FromEmailAddressName, ConfigurationsHelper.SendGrid_FromEmailAddress, ConfigurationsHelper.AdminEmailAddress, EmailTextHelpers.OrderPlacedEmailSubject_Admin(AppDataHelper.CurrentLanguage.ID, newOrder.ID), EmailTextHelpers.OrderPlacedEmailBody_Admin(AppDataHelper.CurrentLanguage.ID, newOrder.ID, Url.Action("Details", "Orders", new { area = "Dashboard", ID = newOrder.ID }, protocol: Request.Url.Scheme)));
                        }

                        jsonResult.Data = new
                        {
                            Success = true,
                            OrderID = newOrder.ID
                        };
                    }
                    else
                    {
                        jsonResult.Data = new
                        {
                            Success = false,
                            Message = "System can not take any order."
                        };
                    }
                }
                else
                {
                    jsonResult.Data = new
                    {
                        Success = false,
                        Message = "Invalid products in cart."
                    };
                }
            }
            else
            {
                jsonResult.Data = new
                {
                    Success = false,
                    Message = string.Join("\n", ModelState.Values
                                        .SelectMany(x => x.Errors)
                                        .Select(x => x.ErrorMessage))
                };
            }

            return jsonResult;
        }

        public ActionResult Tracking(int? orderID, bool orderPlaced = false)
        {
            TrackOrderViewModel model = new TrackOrderViewModel
            {
                OrderID = orderID
            };

            if (orderID.HasValue)
            {
                model.Order = OrdersService.Instance.GetOrderByID(orderID.Value);
            }

            ViewBag.ShowOrderPlaceMessage = orderPlaced;

            return View(model);
        }

        public ActionResult UserOrders(string userID, int? orderID, int? orderStatus, int? pageNo)
        {
            var pageSize = (int)RecordSizeEnums.Size10;

            UserOrdersViewModel model = new UserOrdersViewModel
            {
                UserID = !string.IsNullOrEmpty(userID) ? userID : User.Identity.GetUserId(),
                OrderID = orderID,
                OrderStatus = orderStatus
            };

            model.UserOrders = OrdersService.Instance.SearchOrders(model.UserID, model.OrderID, model.OrderStatus, pageNo, pageSize, count: out int ordersCount);

            model.Pager = new Pager(ordersCount, pageNo, pageSize);

            return PartialView("_UserOrders", model);
        }

        [HttpGet]
        public async Task<ActionResult> Details(int? ID)
        {
            Areas.Dashboard.ViewModels.OrderDetailsViewModel model = new Areas.Dashboard.ViewModels.OrderDetailsViewModel();

            if (ID.HasValue)
            {
                model.Order = OrdersService.Instance.GetOrderByID(ID.Value);
                if (model.Order == null) return HttpNotFound();// if order is removed then this will show 404 error
                
                model.Products = ProductsService.Instance.GetProductsByIDs(model.Order.OrderItems.Select(x=>x.ProductID).ToList());

              

                if (!string.IsNullOrEmpty(model.Order.CustomerID))
                {
                    model.Customer = await UserManager.FindByIdAsync(model.Order.CustomerID);
                }
                
            }
            else
            {
                return HttpNotFound();
            }
            if (Request.IsAjaxRequest())
            {
                return PartialView("_UserOrderDetails", model);
            }
            else return View("UserOrderDetails", model);
           
        }



        public JsonResult RETURNREQUEST(int OrderID, int OrderFncd, int ProductID, decimal weight, string Review, decimal Product_Price, decimal subtotal, int quantity)
        {
            JsonResult json = new JsonResult();
            var mailid = string.Empty;
            var message = string.Empty;
            var UserName = string.Empty;
            var PhoneNumber = string.Empty;
            var _ProductName = string.Empty;
            var UsersDetails = UserProfileService.Instance.GetUser(User.Identity.GetUserId());
            var ProductName = ProductsService.Instance.GetProductRecordByID(ProductID);


            ReturnRequest ReturnItem = new ReturnRequest
            {
                ReturnDate = DateTime.Now,
                OrderID = OrderID,
                OrderFncd = OrderFncd,
                ProductID = ProductID,
                Weight = weight,
                ReturnDetail = Review,
                ProductPrice = Product_Price,
                subtotal = subtotal,
                quantity = quantity

            };
            OrdersService.Instance.ReturnItem(ReturnItem);
            message = "Our Courier partner will pick up the item from your doorstep!";
            json.Data = new { Success = true, Message = message };
            if (UsersDetails != null)
            {
                _ProductName = ProductName.Name;
                UserName = UsersDetails.UserName;
                PhoneNumber = UsersDetails.PhoneNumber;
                string Msg = string.Empty;
                mailid = UsersDetails.Email;
                Msg = "Your " + _ProductName + " is canceled." +
                    " Our Courier partner will pick up the item from your doorstep.";
                NotificationHelper.SendSms(PhoneNumber, Msg, _ProductName);
                NotificationHelper.SendMail(mailid, UserName, Msg);
            }
            return json;
        }
       
        public JsonResult CancelRequest(int OrderID, int OrderFncd, int ProductID, decimal weight, string Review,decimal Product_Price,decimal subtotal, int quantity)
        {
            JsonResult json = new JsonResult();
            var mailid = string.Empty;
            var message = string.Empty;
            var UserName = string.Empty;
            var PhoneNumber = string.Empty;
            var _ProductName = string.Empty;
            var UsersDetails = UserProfileService.Instance.GetUser(User.Identity.GetUserId());
            var ProductName = ProductsService.Instance.GetProductRecordByID(ProductID);

            CancelRequest CancelItem = new CancelRequest
            {
                CancelDate = DateTime.Now,
                OrderID = OrderID,
                OrderFncd = OrderFncd,
                ProductID = ProductID,
                Weight = weight,
                CancelDetail = Review,
                ProductPrice= Product_Price,
                subtotal= subtotal,
                quantity=quantity

            };
            OrdersService.Instance.CancelItem(CancelItem);
            message = "Our Courier partner will pick up the item from your doorstep!";
            json.Data = new { Success = true, Message = message };
            if (UsersDetails != null)
            {
                _ProductName = ProductName.Name;
                UserName = UsersDetails.UserName;
                PhoneNumber = UsersDetails.PhoneNumber;
                mailid = UsersDetails.Email;
                string Msg = string.Empty;
                Msg = "Your order return request for "+ _ProductName + " is accepted." +
                    " Our Courier partner will pick up the item from your doorstep.";
               NotificationHelper.SendSms(PhoneNumber, Msg, _ProductName);
                NotificationHelper.SendMail(mailid,UserName,Msg);
            }
            return json;
        }


        public ActionResult PrintInvoice(int orderID)
        {
            PrintInvoiceViewModel model = new PrintInvoiceViewModel
            {
                OrderID = orderID,

                Order = OrdersService.Instance.GetOrderByID(orderID)
            };

            if (model.Order == null)
            {
                return HttpNotFound();
            }

            return PartialView("_PrintInvoice", model);
        }
    }
}