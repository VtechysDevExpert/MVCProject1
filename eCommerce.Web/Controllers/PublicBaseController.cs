using eCommerce.Shared.Attributes;
using eCommerce.Shared.Helpers;
using System.Web.Mvc;

namespace eCommerce.Web.Controllers
{
    [PageDetail]
    public class PublicBaseController : Controller
    {
        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            AppDataHelper.Populate();

            base.OnActionExecuting(filterContext);
        }
    }
}