using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Assg.Controllers.AdminControllers
{
    public class AdminBaseController : Controller
    {
        public override ViewResult View(string? viewName, object? model)
        {
            ViewData["Layout"] = "~/Views/Shared/_AdminLayout.cshtml";
            return base.View(viewName, model);
        }

        public override ViewResult View(object? model)
        {
            ViewData["Layout"] = "~/Views/Shared/_AdminLayout.cshtml";
            return base.View(model);
        }

        public override ViewResult View(string? viewName)
        {
            ViewData["Layout"] = "~/Views/Shared/_AdminLayout.cshtml";
            return base.View(viewName);
        }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            base.OnActionExecuting(context);
        }
    }
}
