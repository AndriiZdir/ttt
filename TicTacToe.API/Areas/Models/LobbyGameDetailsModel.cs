using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TicTacToe.API.Areas.Models
{
    public class LobbyGameDetailsModel
    {
        public string GameId { get; set; }
        public int MaxUsers { get; set; }
        public int JoinedUsers { get; set; }
        public int MinesQuantity { get; set; }
        public bool IsWithPassword { get; set; }

        public IEnumerable<LobbyGameDetails_Player> Players { get; set; }

        public class LobbyGameDetails_Player
        {
            public string UserId { get; set; }
            public string UserName { get; set; }
            public byte Sign { get; set; }
            public bool IsReady { get; set; }
            public bool IsCreator { get; set; }
        }
    }

    
}
