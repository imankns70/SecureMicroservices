using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Routing;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Movies.API
{
    public class MyOwnAuthorizationRequirement : IAuthorizationRequirement
    {

    }
    public class MyOwnAuthorizationHandler : AuthorizationHandler<MyOwnAuthorizationRequirement>
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        public MyOwnAuthorizationHandler(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        protected override async Task HandleRequirementAsync(
            AuthorizationHandlerContext context, MyOwnAuthorizationRequirement requirement)
        {

            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            var routeData = _httpContextAccessor.HttpContext?.GetRouteData();

            var areaName = routeData?.Values["area"]?.ToString();
            var area = string.IsNullOrWhiteSpace(areaName) ? string.Empty : areaName;

            var controllerName = routeData?.Values["controller"]?.ToString();
            var controller = string.IsNullOrWhiteSpace(controllerName) ? string.Empty : controllerName;

            var actionName = routeData?.Values["action"]?.ToString();
            var action = string.IsNullOrWhiteSpace(actionName) ? string.Empty : actionName;

            var uri = $"{areaName}/{controllerName}/{actionName}";
            var subscriptionClaim = context.User.Claims.FirstOrDefault(claim => claim.Type == "subscriptionlevel");

            if (subscriptionClaim == null)
            {
                context.Fail();
                return;
            }

            if (subscriptionClaim.Value != "a1" && subscriptionClaim.Value != "b1")
            {
                context.Fail();
                return;
            }


            context.Succeed(requirement);
        }
    }
}
