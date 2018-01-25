using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TicTacToe.DAL.Services
{
    public static class ServicesExtensions
    {
        public static IQueryable<T> QuerySkipTake<T>(this IQueryable<T> query, int skip, int take)
        {
            return query.Skip(skip).Take(take);
        }
    }
}
