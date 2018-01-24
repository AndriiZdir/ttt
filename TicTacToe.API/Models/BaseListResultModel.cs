using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TicTacToe.API.Models
{
    public class BaseListResultModel<T> : BaseApiModel
    {
        public BaseListResultModel(IEnumerable<T> list)
        {
            Result = list;
        }

        public IEnumerable<T> Result { get; private set; }
    }
}
