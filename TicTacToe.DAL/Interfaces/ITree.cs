using System;
using System.Collections.Generic;
using System.Linq;

namespace TicTacToe.DAL.Interfaces
{
    /// <summary>
    /// Интерфейс сущности, которая может входить в древовидную структуру. Унаследован от <seealso cref="IEntity"/>. 
    /// </summary>
    public interface ITree : IEntity
    {
        int ParentId { get; set; }
        int Depth { get; set; }
        int cLeft { get; set; }
        int cRight { get; set; }
    }
}