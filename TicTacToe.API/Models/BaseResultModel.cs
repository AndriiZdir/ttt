using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TicTacToe.API.Models
{
    public class BaseResultModel<T> : BaseApiModel
    {
        public BaseResultModel(T item)
        {
            Result = item;
        }

        public T Result { get; private set; }
    }
}
