using eCommerce.Entities;
using eCommerce.Entities.CustomEntities;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace eCommerce.Services
{
   public class CustomerReviewService
    {
        #region Define as Singleton
        private static CustomerReviewService _Instance;

        public static CustomerReviewService Instance
        {
            get
            {
                if (_Instance == null)
                {
                    _Instance = new CustomerReviewService();
                }

                return (_Instance);
            }
        }

        private CustomerReviewService()
        {
        }

        #endregion

        public bool UpdateReview(CustomerReviews _CustomerReviews)
        {
            var context = DataContextHelper.GetNewContext();

            context.Entry(_CustomerReviews).State = System.Data.Entity.EntityState.Modified;

            return context.SaveChanges() > 0;
        }
        //get by id
        public CustomerReviews GetReviewsByID(int ID)
        {
            var context = DataContextHelper.GetNewContext();

            return context.CustomerReviews.FirstOrDefault(x => !x.IsDeleted && x.ID == ID);
        }
        public bool AddReview(CustomerReviews CustomerReview)
        {
            var context = DataContextHelper.GetNewContext();

            context.CustomerReviews.Add(CustomerReview);

            return context.SaveChanges() > 0;
        }

        public List<CustomerReviews> GetReviews(int? pageNo = 1, int? recordSize = 0)
        {
            var context = DataContextHelper.GetNewContext();

            var Reviews = context.CustomerReviews
                                 .Where(x => !x.IsDeleted)
                                 .OrderByDescending(x => x.TimeStamp)
                                 .AsQueryable();
            if (recordSize.HasValue && recordSize.Value > 0)
            {
                pageNo = pageNo ?? 1;
                var skip = (pageNo.Value - 1) * recordSize.Value;

                Reviews = Reviews.Skip(skip)
                                       .Take(recordSize.Value);
            }
           
            return Reviews.ToList();

        }

        public List<CustomerReviews> SearchReview(string searchTerm, int? pageNo, int recordSize, out int count)
        {
            var context = DataContextHelper.GetNewContext();

            var Reviews = context.CustomerReviews
                                .Where(x => !x.IsDeleted)
                                .AsQueryable();

            if (!string.IsNullOrEmpty(searchTerm))
            {
                Reviews = Reviews.Where(x => x.ReviewHeading.ToLower().Contains(searchTerm.ToLower()));
            }

            count = Reviews.Count();

            pageNo = pageNo ?? 1;
            var skipCount = (pageNo.Value - 1) * recordSize;

            return Reviews.OrderByDescending(x => x.TimeStamp).Skip(skipCount).Take(recordSize).ToList();
        }
        public bool DeleteReview(int ID)
        {
            var context = DataContextHelper.GetNewContext();

            var Review = context.CustomerReviews.Find(ID);

            Review.IsDeleted = true;

            context.Entry(Review).State = System.Data.Entity.EntityState.Modified;

            return context.SaveChanges() > 0;
        }



    }





}
