using AuthorizeNet.Api.Contracts.V1;
using eCommerce.Entities;
using eCommerce.Services;
using eCommerce.Shared.Enums;
using eCommerce.Shared.Extensions;
using eCommerce.Shared.Helpers;
using eCommerce.Web.ViewModels;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace eCommerce.Web.Controllers
{
    public class PaymentController : PublicBaseController
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

        public PaymentController()
        {
        }

        public PaymentController(eCommerceUserManager userManager, eCommerceSignInManager signInManager, eCommerceRoleManager roleManager)
        {
            UserManager = userManager;
            SignInManager = signInManager;
            RoleManager = roleManager;
        }

      


        public ActionResult CreateOrder()
        {
            var OrderDetails = OrdersService.Instance.GetOrderByUserID(User.Identity.GetUserId());
            // Generate random receipt number for order
            Random randomObj = new Random();
           
            string transactionId = randomObj.Next(10000000, 100000000).ToString();

            Razorpay.Api.RazorpayClient client = new Razorpay.Api.RazorpayClient("aaaa", "kkkk");
            Dictionary<string, object> options = new Dictionary<string, object>();
            options.Add("amount",OrderDetails.FinalAmmount * 100);  // Amount will in paise
            options.Add("receipt", transactionId);
            options.Add("currency", "INR");
            options.Add("payment_capture", "0"); 
            // 1 - automatic  , 0 - manual
                                                 //options.Add("notes", "-- You can put any notes here --");
            Razorpay.Api.Order orderResponse = client.Order.Create(options);
            string orderId = orderResponse["id"].ToString();

            // //Create order model for return on view
            OrderModel orderModel = new OrderModel
            {
                orderId = orderResponse.Attributes["id"],
                razorpayKey = "aaaa",
                amount = Convert.ToInt32(OrderDetails.FinalAmmount),
                currency = "INR",
                name = OrderDetails.CustomerName,
                email = OrderDetails.CustomerEmail,
                contactNumber = OrderDetails.CustomerPhone,
                address = OrderDetails.CustomerAddress,
                description = "Testing description",
                UserOrderId = OrderDetails.ID,
                Fncd=OrderDetails.Fncd
            };

            // Return on PaymentPage with Order data
            return View("PaymentPage", orderModel);
        }

       


        [HttpPost]
        public ActionResult Complete()
        {
            // Payment data comes in url so we have to get it from url
            
            int id = 0;
            decimal amnt_deci = 0;
            int _fncd = 0;
            string CardName = null;string last4 =null;
            CultureInfo culture = new CultureInfo("en-US");
            
            // This id is razorpay unique payment id which can be use to get the payment details from razorpay server
           
            string paymentId = Request.Params["aaaa"];
            
            string UsersOrderID = Request.Params["userorderid"];
            
            string Fncd= Request.Params["Fncd"];
            
            int.TryParse(UsersOrderID,out id);
            
            int.TryParse(Fncd, out _fncd);
           
            // This is orderId
            string orderId = Request.Params["rzp_orderid"];
            
            Razorpay.Api.RazorpayClient client = new Razorpay.Api.RazorpayClient("aaaa", "kkkk");

            Razorpay.Api.Payment payment = client.Payment.Fetch(paymentId);

            // This code is for capture the payment 
            Dictionary<string, object> options = new Dictionary<string, object>();
            options.Add("amount", payment.Attributes["amount"]);
            Razorpay.Api.Payment paymentCaptured = payment.Capture(options);
            string amt = paymentCaptured.Attributes["amount"];
            string trim_amt = amt.Substring(0, amt.Length - 2);
            decimal.TryParse(trim_amt, out amnt_deci);
            //getting child value of json data
            var card = paymentCaptured.Attributes["card"];
            if (card !=null)
            {
                dynamic card_details = JsonConvert.DeserializeObject(card.ToString());
                 CardName = card_details.network.ToString();
                 last4 = card_details.last4.ToString();

            }

            OrderPaymentDetails paymentDetails = new OrderPaymentDetails()
            {
                OrderID = id,
                OrderFncd = _fncd,
                TransactionID = orderId,
                PaymentID = paymentId,
                PaymentMethod = paymentCaptured.Attributes["method"],
                FinalAmmount = amnt_deci,
                CardName = CardName,
                CardNo = last4,
                EntryOn = DateTime.Now,
                MoneyReceiptFncd= _fncd,
                MoneyReceiptNo=0 

            };

            //// Check payment made successfully
            if (paymentCaptured.Attributes["status"] == "captured")
            {
                SessionHelper.ClearCart();
                //we are storing orderid as transaction id on order table.

                OrdersService.Instance.paymentupdate(id, orderId,true);
                //saving payment details into database
                OrdersService.Instance.SavePaymentDetails(paymentDetails);
                // redirect to user profile these action method
                return RedirectToRoute("OrderSuccess");
            }
            else
            {
                OrdersService.Instance.paymentupdate(id, orderId, false);
                return RedirectToAction("Failed");
            }
        }
        public ActionResult Success()
        {
            OrderModel _Model = new OrderModel();
            return View("Success",_Model);
        }

        public ActionResult Failed()
        {
            return View();
        }
    }
}