using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TicTacToe.DAL.Interfaces
{
    public interface INotDeletable
    {
        bool IsDeleted { get; set; }
    }
}
