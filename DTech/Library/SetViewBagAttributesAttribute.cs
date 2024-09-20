using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace DTech.Library
{
    public class SetViewBagAttributesAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            // Cast to Controller
            var controller = context.Controller as Controller;

            if (controller != null)
            {
                controller.ViewBag.Status = new List<SelectListItem>
                {
                    new() { Value = "1", Text = "Available" },
                    new() { Value = "0", Text = "Unavailable" },
                };

                controller.ViewBag.StatusProduct = new List<SelectListItem>
                {
                    new() { Value = "True", Text = "In stock" },
                    new() { Value = "False", Text = "Out of stock" },
                };
            }

            base.OnActionExecuting(context);
        }
    }
}
