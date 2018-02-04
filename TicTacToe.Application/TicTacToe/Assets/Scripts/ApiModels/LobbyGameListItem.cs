using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Scripts.ApiModels
{
    public class LobbyGameListItem
    {
        public string GameId;
        public string CreatedByUserName;
        public int MaxUsers;
        public int JoinedUsers;
        public int MinesQuantity;
        public bool IsWithPassword;
    }
}
