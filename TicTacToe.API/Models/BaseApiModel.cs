using Microsoft.AspNetCore.Mvc.ModelBinding;
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
    }
}
