using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eCommerce.Entities
{
   public class UserAddress
    {
        public int Id { get; set; }
        public string UserId { get; set; }

        public string Address1 { get; set; }

        public string City { get; set; }

        public int StateId { get; set; }
        public string Country { get; set; }

        public int Zipcode { get; set; }

        public bool IsRemoved { get; set; }
    }
}
