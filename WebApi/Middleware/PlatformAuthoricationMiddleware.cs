using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;

namespace OrderApi.Middleware
{
    public class PlatformAuthoricationMiddleware
    {
        private readonly RequestDelegate _next;
        
        public PlatformAuthoricationMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            Console.WriteLine("invoke");
            await _next.Invoke(context);
        }
    }

    public static class PlatformAuthoricationMiddlewareExtensions
    {
        public static IApplicationBuilder UsePlatformAuthorication(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<PlatformAuthoricationMiddleware>();
        }
    }

    public class PlatformAuthenticationOptions: AuthenticationOptions
    {

    }
}
