namespace eCommerce.Web.ViewModels
{
    public class PriceRangeFilterViewModel
    {
        public decimal? PriceFrom { get; set; }
        public decimal? PriceTo { get; set; }
        public decimal MaxPrice { get; set; }
    }
}