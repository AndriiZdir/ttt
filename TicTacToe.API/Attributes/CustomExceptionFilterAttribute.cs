using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TicTacToe.API.Models;

namespace TicTacToe.API.Attributes
{

    public class CustomExceptionFilterAttribute : ExceptionFilterAttribute
    {
        public override void OnException(ExceptionContext context)
        {
            var exception = context.Exception;
            context.HttpContext.Response.StatusCode = 500;
            context.Result = new JsonResult(new BaseApiModel { Code = 500, Message = exception.Message });
        }        
    }
}
