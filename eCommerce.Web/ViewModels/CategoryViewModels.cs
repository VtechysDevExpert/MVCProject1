using eCommerce.Entities.CustomEntities;
using System.Collections.Generic;
using eCommerce.Entities;
namespace eCommerce.Web.ViewModels
{
    public class CategoriesMenuViewModel
    {
        public List<Category> Categories { get; set; }
        public List<CategoryWithChildren> CategoryWithChildrens { get; set; }
        public List<CategoryWithSubChildren> CategoryWithsSubChildrens { get; set; }

        public List<ProductPicture> ProductPicture { get; set; }
    }
 
}