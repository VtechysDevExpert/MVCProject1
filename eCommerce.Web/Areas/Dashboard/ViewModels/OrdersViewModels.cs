﻿using eCommerce.Entities;
using eCommerce.Web.ViewModels;
using System.Collections.Generic;

namespace eCommerce.Web.Areas.Dashboard.ViewModels
{
    public class OrdersListingViewModel : PageViewModel
    {
        public List<Order> Orders { get; set; }

        public string UserEmail { get; set; }
        public int? OrderID { get; set; }
        public int? OrderStatus { get; set; }

        public Pager Pager { get; set; }
        public string UserID { get; set; }
    }

    public class OrderDetailsViewModel : PageViewModel
    {
        public Order Order { get; set; }
        public eCommerceUser Customer { get; set; }
        public List<Product> Products { get; set; }
    }
}