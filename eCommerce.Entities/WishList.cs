using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eCommerce.Entities
{
    public class WishList
    {
        [Key]
        public int ID { get; set; }
        public int ItemID { get; set; }
        public string UserID { get; set; }

        public bool IsRemoved { get; set; }
    }
}
