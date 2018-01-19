using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace TicTacToe.API.Areas.Game
{
    [Produces("application/json")]
    [Area("g")]
    public class BaseGameController : Controller
    {
        protected IHostingEnvironment hostEnvironment { get; set; }

        public BaseGameController(IHostingEnvironment hostEnvironment)
        {
            this.hostEnvironment = hostEnvironment;
        }
    }
}