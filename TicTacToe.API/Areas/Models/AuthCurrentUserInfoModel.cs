using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TicTacToe.API.Areas.Models
{
    public class AuthCurrentUserInfoModel
    {
        public string PlayerName { get; set; }

        public string PlayerId { get; set; }
    }
}
