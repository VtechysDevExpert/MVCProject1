using System;

namespace eCommerce.Entities
{
    public class BaseEntity
    {
        public int ID { get; set; }

        public bool IsActive { get; set; }

        public bool IsDeleted { get; set; }

        public DateTime? ModifiedOn { get; set; }
    }
}
