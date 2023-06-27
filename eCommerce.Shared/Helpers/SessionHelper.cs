#pragma warning disable CS0234 // The type or namespace name 'Entities' does not exist in the namespace 'eCommerce' (are you missing an assembly reference?)
using eCommerce.Entities;
#pragma warning restore CS0234 // The type or namespace name 'Entities' does not exist in the namespace 'eCommerce' (are you missing an assembly reference?)
#pragma warning disable CS0234 // The type or namespace name 'Entities' does not exist in the namespace 'eCommerce' (are you missing an assembly reference?)
using eCommerce.Entities.CustomEntities;
#pragma warning restore CS0234 // The type or namespace name 'Entities' does not exist in the namespace 'eCommerce' (are you missing an assembly reference?)
using System.Collections.Generic;

namespace eCommerce.Shared.Helpers
{
    public static class SessionHelper
    {
        private const string CART = "CART";
        private const string CART_ITEMS = "CART_ITEMS";
        private const string PROMO = "PROMO";
        private const string PROMO_CODE = "PROMO_CODE";
        private const string DARK_MODE = "DARK_MODE";

#pragma warning disable CS0246 // The type or namespace name 'Cart' could not be found (are you missing a using directive or an assembly reference?)
        public static Cart Cart
#pragma warning restore CS0246 // The type or namespace name 'Cart' could not be found (are you missing a using directive or an assembly reference?)
        {
            get
            {
                var cart = SessionManager.Get<Cart>(CART);

                if (cart == null)
                {
                    cart = new Cart();

                    SessionManager.Set(CART, cart);
                }

                return cart;
            }
            set { SessionManager.Set(CART, value); }
        }

#pragma warning disable CS0246 // The type or namespace name 'CartItem' could not be found (are you missing a using directive or an assembly reference?)
        public static List<CartItem> CartItems
#pragma warning restore CS0246 // The type or namespace name 'CartItem' could not be found (are you missing a using directive or an assembly reference?)
        {
            get
            {
                var cartItems = SessionManager.Get<List<CartItem>>(CART_ITEMS);

                if (cartItems == null || cartItems.Count == 0)
                {
                    cartItems = cartItems == null ? new List<CartItem>() : cartItems;

                    SessionManager.Set(CART_ITEMS, cartItems);
                }

                return cartItems;
            }
            set { SessionManager.Set(CART_ITEMS, value); }
        }

#pragma warning disable CS0246 // The type or namespace name 'Promo' could not be found (are you missing a using directive or an assembly reference?)
        public static Promo Promo
#pragma warning restore CS0246 // The type or namespace name 'Promo' could not be found (are you missing a using directive or an assembly reference?)
        {
            get
            {
                return SessionManager.Get<Promo>(PROMO);
            }
            set { SessionManager.Set(PROMO, value); }
        }
        public static string PromoCode
        {
            get
            {
                var promoCode = SessionManager.Get<string>(PROMO_CODE);

                if (string.IsNullOrEmpty(promoCode))
                {
                    promoCode = string.Empty;

                    SessionManager.Set(PROMO_CODE, promoCode);
                }

                return promoCode;
            }
            set { SessionManager.Set(PROMO_CODE, value); }
        }

        public static void ClearCart()
        {
            CartItems.Clear();
            PromoCode = string.Empty;
            Promo = null;
        }

        public static string DarkMode
        {
            get
            {
                var darkMode = SessionManager.Get<string>(DARK_MODE);

                if (string.IsNullOrEmpty(darkMode))
                {
                    darkMode = "true";

                    SessionManager.Set(DARK_MODE, darkMode);
                }

                return darkMode;
            }
            set { SessionManager.Set(DARK_MODE, value); }
        }
    }
}
