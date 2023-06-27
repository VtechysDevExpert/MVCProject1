using eCommerce.Entities;
using eCommerce.Entities.CustomEntities;
using System.Collections.Generic;

namespace eCommerce.Web.ViewModels
{
    public class CartItemsViewModel : PageViewModel
    {
        public List<CartItem> CartItems { get; set; }
        public List<int> ProductIDs { get; set; }
        public List<Product> Products { get; set; }
        public List<ProductDetails> ProductDetails_vm { get; set; }
        public string PromoCode { get; set; }
        public Promo Promo { get; set; }
    }

    public class UpdateCartItemsViewModel : PageViewModel
    {
        public List<CartItem> CartItems { get; set; }
        public string PromoCode { get; set; }
    }

    public class CheckoutViewModel : CartItemsViewModel
    {public List<UserAddress> User_Addresses { get; set; }
        public bool CartHasProducts { get; set; }
        public List<State> states { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal Discount { get; set; }
        public bool PromoApplied { get; set; }

        public eCommerceUser User { get; set; }
        public decimal FinalAmount { get; set; }
    }

    public class ConfirmOrderViewModel
    {
        public List<int> ProductIDs { get; set; }
        public List<Product> Products { get; set; }
    }
}