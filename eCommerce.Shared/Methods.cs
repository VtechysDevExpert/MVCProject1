#pragma warning disable CS0234 // The type or namespace name 'Entities' does not exist in the namespace 'eCommerce' (are you missing an assembly reference?)
using eCommerce.Entities;
#pragma warning restore CS0234 // The type or namespace name 'Entities' does not exist in the namespace 'eCommerce' (are you missing an assembly reference?)
using System.Collections.Generic;
using System.Linq;

namespace eCommerce.Shared
{
    public class Methods
    {
#pragma warning disable CS0246 // The type or namespace name 'Category' could not be found (are you missing a using directive or an assembly reference?)
#pragma warning disable CS0246 // The type or namespace name 'Category' could not be found (are you missing a using directive or an assembly reference?)
#pragma warning disable CS0246 // The type or namespace name 'Category' could not be found (are you missing a using directive or an assembly reference?)
        public static List<Category> GetCategoryHierarchy(Category category, List<Category> allCategories)
#pragma warning restore CS0246 // The type or namespace name 'Category' could not be found (are you missing a using directive or an assembly reference?)
#pragma warning restore CS0246 // The type or namespace name 'Category' could not be found (are you missing a using directive or an assembly reference?)
#pragma warning restore CS0246 // The type or namespace name 'Category' could not be found (are you missing a using directive or an assembly reference?)
        {
            if (category != null && allCategories != null && allCategories.Count > 0)
            {
                var categories = new List<Category>() { category };

                Category parentCategory = null;

                var parentCategoryID = category.ParentCategoryID;

                do
                {
                    parentCategory = GetCategoryParent(parentCategoryID, allCategories);

                    if (parentCategory != null)
                    {
                        categories.Add(parentCategory);

                        parentCategoryID = parentCategory.ParentCategoryID;
                    }
                    else
                    {
                        parentCategoryID = null;
                    }
                } while (parentCategory != null);

                categories.Reverse();

                return categories;
            }

            return null;
        }

#pragma warning disable CS0246 // The type or namespace name 'Category' could not be found (are you missing a using directive or an assembly reference?)
#pragma warning disable CS0246 // The type or namespace name 'Category' could not be found (are you missing a using directive or an assembly reference?)
        public static Category GetCategoryParent(int? parentCategoryID, List<Category> allCategories)
#pragma warning restore CS0246 // The type or namespace name 'Category' could not be found (are you missing a using directive or an assembly reference?)
#pragma warning restore CS0246 // The type or namespace name 'Category' could not be found (are you missing a using directive or an assembly reference?)
        {
            return parentCategoryID.HasValue ? allCategories.FirstOrDefault(x => x.ID == parentCategoryID) : null;
        }

#pragma warning disable CS0246 // The type or namespace name 'Category' could not be found (are you missing a using directive or an assembly reference?)
#pragma warning disable CS0246 // The type or namespace name 'Category' could not be found (are you missing a using directive or an assembly reference?)
#pragma warning disable CS0246 // The type or namespace name 'Category' could not be found (are you missing a using directive or an assembly reference?)
        public static List<Category> GetAllCategoryChildrens(Category category, List<Category> allCategories)
#pragma warning restore CS0246 // The type or namespace name 'Category' could not be found (are you missing a using directive or an assembly reference?)
#pragma warning restore CS0246 // The type or namespace name 'Category' could not be found (are you missing a using directive or an assembly reference?)
#pragma warning restore CS0246 // The type or namespace name 'Category' could not be found (are you missing a using directive or an assembly reference?)
        {
            if (category != null && allCategories != null && allCategories.Count > 0)
            {
                var categories = new List<Category>() { category };

                var childCategories = GetCategoryChildren(category.ID, allCategories);

                foreach (var childCategory in childCategories)
                {
                    categories.Add(childCategory);

                    GetAllCategoryChildrens(childCategory, allCategories);
                }

                return categories;
            }

            return null;
        }

#pragma warning disable CS0246 // The type or namespace name 'Category' could not be found (are you missing a using directive or an assembly reference?)
#pragma warning disable CS0246 // The type or namespace name 'Category' could not be found (are you missing a using directive or an assembly reference?)
        public static List<Category> GetCategoryChildren(int parentCategoryID, List<Category> allCategories)
#pragma warning restore CS0246 // The type or namespace name 'Category' could not be found (are you missing a using directive or an assembly reference?)
#pragma warning restore CS0246 // The type or namespace name 'Category' could not be found (are you missing a using directive or an assembly reference?)
        {
            return allCategories.Where(x => x.ParentCategoryID == parentCategoryID).ToList();
        }
    }
}
