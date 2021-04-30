using System.Linq;
using System.Net;
using System.Reflection;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Controllers;

namespace WebApplication.Extensions
{
    public static class StartupExtensions
    {
        public static IApplicationBuilder UseMySimpleAuthorization(this IApplicationBuilder app)
        {
            app.Use(async (context, next) =>
            {
                var controllerActionDescriptor = context
                    .GetEndpoint()
                    ?.Metadata
                    .GetMetadata<ControllerActionDescriptor>();

                var actionName = controllerActionDescriptor?.ActionName;

                var method = controllerActionDescriptor
                    ?.ControllerTypeInfo
                    ?.GetMethods()
                    .FirstOrDefault(x => x.Name == actionName);

                var attr = method?
                    .GetCustomAttributes<AuthorizeAttribute>(true)
                    .FirstOrDefault();
                
                if (attr != null && !context.Request.Headers.ContainsKey("Authorization"))
                {
                    context.Response.StatusCode = (int) HttpStatusCode.Forbidden;
                    await context.Response.CompleteAsync();
                    return;
                }
                await next();
            });

            return app;
        }
    }
}