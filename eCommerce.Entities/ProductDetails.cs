using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eCommerce.Entities
{
   public class ProductDetails
    {
        [Key]
        public int ID { get; set; }

        public int ProductID { get; set; }
        public int StockQuantity { get; set; }

        public decimal Weight { get; set; }

        public decimal price { get; set; }
      
        public decimal? Discount { get; set; }
       
        public decimal Amount { get; set; }

        public DateTime? ModifiedOn { get; set; }
      
        public bool IsActive { get; set; }
       
        public bool IsDeleted { get; set; }
    }
}
