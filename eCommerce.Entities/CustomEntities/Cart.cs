using System.Collections.Generic;

namespace eCommerce.Entities.CustomEntities
{
    public class Cart
    {
        public List<CartItem> CartItems { get; set; } = new List<CartItem>();
    }
}
