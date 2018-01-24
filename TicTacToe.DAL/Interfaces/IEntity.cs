using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TicTacToe.DAL.Interfaces
{
    /// <summary>
    /// Интерфейс сущности с идентификатором.
    /// </summary>
    public interface IEntity
    {
        int Id { get; set; }
    }
}
