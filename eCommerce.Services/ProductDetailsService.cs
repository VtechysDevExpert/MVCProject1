using eCommerce.Entities;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eCommerce.Services
{
  public  class ProductDetailsService
    {
        #region Define as Singleton
        private static ProductDetailsService _Instance;

        public static ProductDetailsService Instance
        {
            get
            {
                if (_Instance == null)
                {
                    _Instance = new ProductDetailsService();
                }

                return (_Instance);
            }
        }

        private ProductDetailsService()
        {
        }
        #endregion


        public List<ProductDetails> GetProductDetails()
        {
            var context = DataContextHelper.GetNewContext();

            var productDetails = context.ProductDtls.OrderBy(x => x.ID).AsQueryable();

            

            return productDetails.ToList();
        }




        public ProductDetails GetProductDetailsByID(int? ID)
        {
            var context = DataContextHelper.GetNewContext();

            //var productDetails = context.ProductDtls.Where(X => X.ID == ID && !X.IsDeleted).FirstOrDefault();

            var productDetails = context.ProductDtls.Single(x => x.ID == ID && x.IsDeleted == false);

            return productDetails ;
        }

        public List<ProductDetails> GetProductsByIDs(List<int> IDs)
        {
            var context = DataContextHelper.GetNewContext();
            //var productRcds = IDs.Select(id => context.ProductDtls.Find(id)).Where(x => !x.IsDeleted && x.IsActive).OrderBy(x => x.ID).ToList();
           return context.ProductDtls.Where(x =>IDs.Contains(x.ID)).ToList();
          //  return productRcds.ToList();
        }

        public void UpdateProductQuantities(Order order)
        {
            var context = DataContextHelper.GetNewContext();

            foreach (var orderItem in order.OrderItems)
            {
                var dbProduct = context.ProductDtls.Find(orderItem.ID);

                dbProduct.StockQuantity = dbProduct.StockQuantity - orderItem.Quantity;

                context.Entry(dbProduct).State = EntityState.Modified;
            }

            context.SaveChanges();
        }

    }
}
