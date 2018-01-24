using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TicTacToe.DAL.Interfaces
{
    public interface ILocalization<T> where T : IEntity
    {
        int LanguageId { get; set; }
    }
}
