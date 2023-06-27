using System.Collections.Generic;
namespace eCommerce.Entities.CustomEntities
{
    public class CategoryWithChildren
    {
        public Category Category { get; set; }
        public List<Category> Children { get; set; }
    }
    public class CategoryWithSubChildren
    {
        public Category Category { get; set; }
        public List<Category> Children { get; set; }
    }
}
