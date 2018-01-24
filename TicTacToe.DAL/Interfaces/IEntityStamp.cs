using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TicTacToe.DAL.Interfaces;

namespace TicTacToe.DAL.Interfaces
{

    /// <summary>
    /// Интерфейс сущности с отметкой времени и пользователя, который создал или изменил ее. Унаследован от <seealso cref="IEntity"/>.
    /// </summary>
    public interface IEntityStamp
    {
        DateTime CreateDate { get; set; }
        DateTime LastEditDate { get; set; }
        string CreateUser { get; set; }
        string LastEditUser { get; set; }
    }
}
