using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations.Schema;

namespace eCommerce.Entities
{
    [Table("ReturnRequest")]
    public class ReturnRequest
    {

        public int ID { get; set; }

        public DateTime? ReturnDate { get; set; }
        public int OrderID { get; set; }
        public int OrderFncd { get; set; }

        public int ProductID { get; set; }

        public string ReturnDetail { get; set; }
        public decimal Weight { get; set; }
        public decimal ProductPrice { get; set; }

        public decimal subtotal { get; set; }

        public int quantity { get; set; }

    }
}
