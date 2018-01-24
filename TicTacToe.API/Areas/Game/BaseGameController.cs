using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TicTacToe.API.Attributes;
using TicTacToe.API.Models;

namespace TicTacToe.API.Areas.Game
{
    [Produces("application/json")]
    [CustomExceptionFilter]
    public class BaseGameController : Controller
    {
        //protected IHostingEnvironment hostEnvironment { get; set; }

        //public BaseGameController(IHostingEnvironment hostEnvironment)
        //{
        //    this.hostEnvironment = hostEnvironment;
        //}

        public BaseApiModel ListResult<T>(IEnumerable<T> list)
        {
            return new BaseListResultModel<T>(list);
        }

        public BaseApiModel EntityResult<T>(T entity)
        {
            return new BaseResultModel<T>(entity);
        }

        public BaseApiModel ValidationResult()
        {
            var result = ListResult(ModelState.ToDictionary(x => x.Key, y => y.Value.Errors));
            result.Code = 400;
            result.Message = "Invalid input";
            return result;
        }
    }
}