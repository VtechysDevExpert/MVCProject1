using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace eCommerce.Web.ViewModels
{
    public class PaymentViewModel :PageViewModel
    {

    }
    public class OrderModel : PageViewModel
    {
        public string orderId { get; set; }
        public string razorpayKey { get; set; }
        public int amount { get; set; }
        public string currency { get; set; }
        public string name { get; set; }
        public string email { get; set; }
        public string contactNumber { get; set; }
        public string address { get; set; }
        public string description { get; set; }
        public int UserOrderId { get; set; }
        public int Fncd { get; set; }
    }
}