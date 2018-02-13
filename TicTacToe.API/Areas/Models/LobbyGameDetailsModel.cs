using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TicTacToe.DAL.Models;

namespace TicTacToe.API.Areas.Models
{
    public class LobbyGameDetailsModel
    {
        public string GameId { get; set; }
        public string CreatedBy { get; set; }
        public int MaxPlayers { get; set; }
        public int JoinedPlayers { get; set; }
        public int MinesQuantity { get; set; }
        public bool IsWithPassword { get; set; }
        public GameRoomState GameState { get; set; }

        public IEnumerable<LobbyGameDetails_Player> Players { get; set; }

        public class LobbyGameDetails_Player
        {
            public string PlayerId { get; set; }
            public string PlayerName { get; set; }
            public byte Sign { get; set; }
            public bool IsReady { get; set; }
            public bool IsCreator { get; set; }
        }

        public static LobbyGameDetailsModel LoadFromGameRoom(GameRoom gameRoom)
        {
            var model = new LobbyGameDetailsModel
            {
                GameId = gameRoom.RoomGuid.ToString(),
                CreatedBy = gameRoom.CreateUser,
                MaxPlayers = gameRoom.MaxPlayers,
                JoinedPlayers = gameRoom.GameRoomPlayers.Count,
                MinesQuantity = gameRoom.MinesQuantity,
                IsWithPassword = (gameRoom.Password != null),
                GameState = gameRoom.State,

                Players = gameRoom.GameRoomPlayers.Select(x => new LobbyGameDetails_Player
                {
                    PlayerId = x.UserId,
                    PlayerName = x.Player.UserName,
                    Sign = (byte)x.PlayerSign,
                    IsReady = (x.PlayerState == DAL.Enums.GameRoomPlayerState.Ready),
                    IsCreator = (x.PlayerType == DAL.Enums.GameRoomPlayerType.Creator)
                }).ToList()
            };

            return model;
        }
    }

    
}
