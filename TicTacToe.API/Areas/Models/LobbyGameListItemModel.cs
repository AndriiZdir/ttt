using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TicTacToe.DAL.Models;

namespace TicTacToe.API.Areas.Models
{
    public class LobbyGameListItemModel
    {
        public string GameId { get; set; }
        public string CreatedByUserName { get; set; }
        public int MaxUsers { get; set; }
        public int JoinedUsers { get; set; }
        public int MinesQuantity { get; set; }
        public bool IsWithPassword { get; set; }

        public static System.Linq.Expressions.Expression<Func<GameRoom, LobbyGameListItemModel>> GetSelectExpression()
        {
            return (x => new LobbyGameListItemModel
                    {
                        GameId = x.RoomGuid.ToString(),
                        CreatedByUserName = x.CreatedBy.UserName,
                        IsWithPassword = (x.Password != null),
                        MaxUsers = x.MaxPlayers,
                        JoinedUsers = x.GameRoomPlayers.Count,
                        MinesQuantity = x.MinesQuantity
                    });
        }
    }
}
