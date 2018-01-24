using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TicTacToe.DAL.Interfaces
{
    public interface ILanguage : IEntity
    {
        String Code { get; set; }
    }
}
