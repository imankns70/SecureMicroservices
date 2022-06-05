using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Movies.API
{
    public class MyOwnAuthorizationRequirement : IAuthorizationRequirement
    {

    }
    public class MyOwnAuthorizationHandler : AuthorizationHandler<MyOwnAuthorizationRequirement>
    {
   
        
        protected override async Task HandleRequirementAsync(
            AuthorizationHandlerContext context, MyOwnAuthorizationRequirement requirement)
        {

    
            var filterContext = context.Resource as AuthorizationFilterContext;

            if (filterContext == null)
            {
                context.Fail();
                return;
            }

            var subscriptionClaim = context.User.Claims.FirstOrDefault(claim => claim.Type == "subscriptionlevel");

            if (subscriptionClaim == null)
            {
                context.Fail();
                return;
            }

            if (subscriptionClaim.Value != "a1" || subscriptionClaim.Value != "b1")
            {
                context.Fail();
                return;
            }


            context.Succeed(requirement);
        }
    }
}
