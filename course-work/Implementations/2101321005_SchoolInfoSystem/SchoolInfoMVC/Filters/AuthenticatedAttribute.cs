using Data.Entities;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using Microsoft.IdentityModel.Tokens;

namespace SchoolInfoMVC.Filters
{
    public class AuthenticatedAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            if (context.HttpContext.Request.Cookies["JWT-Access-Token"].IsNullOrEmpty())
            {
                context.Result = new RedirectResult("/Auth/Login");
            }
        }
    }
}
