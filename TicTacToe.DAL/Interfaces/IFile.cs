using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TicTacToe.DAL.Interfaces
{
    public interface IFile : IEntity
    {
        string FileName { get; set; }        
        long FileSize { get; set; }
    }
}
