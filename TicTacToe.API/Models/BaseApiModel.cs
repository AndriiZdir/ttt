using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TicTacToe.API.Models
{
    public class BaseApiModel
    {
        public BaseApiModel()
        {
            Code = 0;
            Message = "success";
        }
        public string Message { get; set; }
        public int Code { get; set; }

        public static BaseApiModel NotFoundResult() { return NotFoundResult("Not found"); }

        public static BaseApiModel NotFoundResult(string message) { return ErrorResult(404, message); }

        public static BaseApiModel ErrorResult(int code, string message) { return new BaseApiModel { Code = code, Message = message }; }
    }
}
