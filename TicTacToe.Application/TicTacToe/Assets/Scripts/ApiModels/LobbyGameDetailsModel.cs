using System;
using System.Collections.Generic;
using System.Linq;

namespace Assets.Scripts.ApiModels
{
    public class LobbyGameDetailsModel
    {
        public string GameId;
        public string CreatedBy;
        public int MaxPlayers;
        public int JoinedPlayers;
        public int MinesQuantity;
        public bool IsWithPassword;
        public GameRoomState GameState;

        public IEnumerable<LobbyGameDetails_Player> Players;

        public class LobbyGameDetails_Player
        {
            public string PlayerId;
            public string PlayerName;
            public byte Sign;
            public bool IsReady;
            public bool IsCreator;
        }        
    }

    public enum GameRoomState
    {
        New,
        Started,
        Closed
    }
}
