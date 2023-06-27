using eCommerce.Entities;
using eCommerce.Web.ViewModels;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;


namespace eCommerce.Web.Areas.Dashboard.ViewModels
{
    public class CustomerReviewListingViewModel :PageViewModel
    {
        public List<CustomerReviews> CustomerReviews { get; set; }
        public string SearchTerm { get; set; }
        public Pager Pager { get; set; }
    }
    public class CustomerReviewActionViewModel : PageViewModel
    {
        public int ID { get; set; }
        [DataType(DataType.Date)]
        public DateTime TimeStamp { get; set; }
        public string UserName { get; set; }
        public int Rating { get; set; }
        public string Description { get; set; }
        public int ProductPictureID { get; set; }
        public string ProductName { get; set; }
        public string ReviewHeading { get; set; }
        public bool IsDeleted { get; set; }
        public string CustomerUrl { get; set; }
        
    }
}
