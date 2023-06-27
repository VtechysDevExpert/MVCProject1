using eCommerce.Entities;
using System.Collections.Generic;

namespace eCommerce.Web.ViewModels
{
    public class HomeSlidersViewModel
    {
        public List<Configuration> SlidersConfigurations { get; set; }
    }
   public class BlogViewModel
    {
        public List<Configuration> BlogConfigurations { get; set; }
    }
    public  class CustomerReviewViewModel
    {
        public List<CustomerReviews> CustomerReviews { get; set; }
    }
   
}