namespace eCommerce.Entities.CustomEntities
{
    public class CartItem
    {
        public int ID { get; set; }
        public int ItemID { get; set; }
        public int Quantity { get; set; }
        public decimal Weight { get; set; }
        public decimal Price { get; set; }
        public decimal ProductTotal
        {
            get
            {
                return Price * Quantity;
            }
        }
    }
}
