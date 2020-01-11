using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.MiddleWare
{
    public class CustomExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        //private ILogger
        public CustomExceptionMiddleware(RequestDelegate next)
        {
            _next = next;
        }


    }
}
