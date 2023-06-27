using eCommerce.Entities;
using eCommerce.Entities.CustomEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace eCommerce.Web.ViewModels
{
    public class WishListViewModels:PageViewModel
    {
        //wishlist class is inside cartitem class in ECommerce.entities
      public  List<WishList> WishLists { get; set; }
        public List<int> ProductIDs { get; set; }
        public List<Product> Products { get; set; }
        public List<ProductDetails> ProductDetails_vm { get; set; }
    }
}