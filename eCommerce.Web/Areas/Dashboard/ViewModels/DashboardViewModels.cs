﻿using eCommerce.Entities;
using eCommerce.Web.ViewModels;
using System.Collections.Generic;

namespace eCommerce.Web.Areas.Dashboard.ViewModels
{
    public class DashboardViewModel : PageViewModel
    {
        public int ProductsCount { get; set; }
        public int OrdersCount { get; set; }
        public int CommentsCount { get; set; }
        public int CategoriesCount { get; set; }
        public int UserCount { get; set; }
        public int RolesCount { get; set; }
        public decimal salesCount { get; set; }
        public decimal productsvalueCount { get; set; }
        public List<Product> ProductsWithLessStockQuantity { get; set; }
    }

    public class NewsletterSubscribersListingViewModel : PageViewModel
    {
        public List<NewsletterSubscription> NewsletterSubscribers { get; set; }

        public string SearchTerm { get; set; }

        public Pager Pager { get; set; }
    }
}