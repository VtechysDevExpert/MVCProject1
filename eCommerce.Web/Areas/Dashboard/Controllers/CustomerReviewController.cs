using eCommerce.Entities;
using eCommerce.Services;
using eCommerce.Shared.Enums;
using eCommerce.Shared.Helpers;
using eCommerce.Web.Areas.Dashboard.ViewModels;
using eCommerce.Web.ViewModels;
using System;
using System.Web.Mvc;

namespace eCommerce.Web.Areas.Dashboard.Controllers
{
    public class CustomerReviewController : DashboardBaseController
    {
        public ActionResult Index(string searchTerm, int? pageNo)
        {//done
            var pageSize = (int)RecordSizeEnums.Size10;

            CustomerReviewListingViewModel model = new CustomerReviewListingViewModel
            {
                SearchTerm = searchTerm
            };

            model.CustomerReviews = CustomerReviewService.Instance.SearchReview(searchTerm, pageNo, pageSize, out int count);

            model.Pager = new Pager(count, pageNo, pageSize);

            return View(model);
        }

        [HttpGet]
        public ActionResult Action(int? ID)
        {
            CustomerReviewActionViewModel model = new CustomerReviewActionViewModel();

            if (ID.HasValue)
            {
                var _review = CustomerReviewService.Instance.GetReviewsByID(ID.Value);

                if (_review == null) return HttpNotFound();

                model.ID = _review.ID;
                model.ReviewHeading = _review.ReviewHeading;
                model.UserName = _review.UserName;
                model.Rating = _review.Rating;
                model.Description = _review.Description;
                model.ProductPictureID = _review.ProductPictureID;
                model.ProductName = _review.ProductName;
                model.CustomerUrl = _review.CustomerUrl;
                model.TimeStamp = _review.TimeStamp;
                
            }

            return View(model);
        }

        [HttpPost, ValidateInput(false)]
        public JsonResult Action(CustomerReviewActionViewModel model)
        {
            JsonResult json = new JsonResult();

            try
            {
                if (model.ID> 0)
                {
                    var _review = CustomerReviewService.Instance.GetReviewsByID(model.ID);

                    if (_review == null)
                    {
                        throw new Exception("Not found!");
                    }

                    _review.ID= model.ID;
                    _review.ReviewHeading= model.ReviewHeading;
                    _review.UserName= model.UserName ;
                    _review.Rating= model.Rating ;
                    _review.Description= model.Description;
                    _review.ProductPictureID= model.ProductPictureID ;
                    _review.ProductName= model.ProductName ;
                    _review.CustomerUrl = model.CustomerUrl;
                    model.TimeStamp = DateTime.Now;

                    if (!CustomerReviewService.Instance.UpdateReview(_review))
                    {
                        throw new Exception("Unable To Update Review.");
                    }

                    json.Data = new { Success = true };
                }
                else
                {
                    CustomerReviews _review = new CustomerReviews
                    {
                       ID = model.ID,
                    ReviewHeading = model.ReviewHeading,
                   UserName  = model.UserName,
                    Rating = model.Rating,
                   Description = model.Description,
                   ProductPictureID  = model.ProductPictureID,
                    ProductName = model.ProductName,
                    CustomerUrl = model.CustomerUrl
                   ,TimeStamp=DateTime.Now
                    };

                    if (!CustomerReviewService.Instance.AddReview(_review))
                    {
                        throw new Exception("Unable To Create Review");
                    }

                    json.Data = new { Success = true };
                }
            }
            catch (Exception ex)
            {
                json.Data = new { Success = false, Message = ex.Message };
            }

            return json;
        }

        [HttpPost]
        public JsonResult Delete(int ID)
        {
            JsonResult result = new JsonResult();

            try
            {
                var operation = CustomerReviewService.Instance.DeleteReview(ID);

                result.Data = new { Success = operation, Message = operation ? string.Empty : "Unable To Delete Review"};
            }
            catch (Exception ex)
            {
                result.Data = new { Success = false, Message = string.Format("{0}", ex.Message) };
            }

            return result;
        }

    }
}