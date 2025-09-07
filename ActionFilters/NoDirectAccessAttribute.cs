using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
namespace SocialMedia.ActionFilters
{


    public class NoDirectAccessAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (filterContext.HttpContext.Request.Headers["Referer"].Count == 0)
            {
                filterContext.Result = new NotFoundResult();
            }
        }
    }

}
