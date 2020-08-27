using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace LoginApi.Filters
{
    public class ApiResultFilterAttribute : ActionFilterAttribute
    {
        public override void OnResultExecuting(ResultExecutingContext context)
        {
            //如果是view control,略过
            if (context.Result.GetType() == typeof(ViewResult))
                return;

            //数据注解验证不通过
            if (!context.ModelState.IsValid)
            {
                //var result = context.ModelState.Keys
                //        .SelectMany(key => context.ModelState[key].Errors.Select(x => new ValidationError(key, x.ErrorMessage)))
                //        .ToList();
                //context.Result = new BadRequestObjectResult(new
                //{
                //    success = false,
                //    errors = result
                //});
            }
        }
    }


    /// <summary>
    /// 异常处理
    /// </summary>
    public class ElevatorExceptionAttribute : IExceptionFilter
    {
        public void OnException(ExceptionContext context)
        {
            HttpStatusCode status = HttpStatusCode.InternalServerError;

            //处理各种异常

            context.ExceptionHandled = true;
            //context.Result = new ElevatorExceptionResult((int)status, context.Exception);
        }
    }
}
