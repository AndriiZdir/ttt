using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TicTacToe.DAL.Interfaces
{
    /// <summary>
    /// Интерфейс сущности, которая может быть отсортирована. Унаследован от <seealso cref="IEntity"/>.
    /// </summary>
    public interface ISortable : IEntity
    {
        int SortOrder { get; set; }
    }
}
