using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using TicTacToe.DAL.Models;

namespace TicTacToe.DAL.Services
{
    public class GameRoomService
    {
        GameDBContext _dbContext;

        public GameRoomService(GameDBContext dbContext)
        {
            _dbContext = dbContext;
        }

        public GameRoom CreateGame(string creatorUserId, int maxPlayers, bool isHidden, string password = null)
        {
            var creatorPlayer = _dbContext.Users.Find(creatorUserId);

            if (creatorPlayer == null) { throw new ArgumentException("There is no such user!"); }



            var gameRoom = new GameRoom();

            gameRoom.RoomGuid = Guid.NewGuid();
            gameRoom.State = GameRoomState.New;
            gameRoom.MaxPlayers = maxPlayers;
            gameRoom.Password = password;
            gameRoom.IsHidden = isHidden;

            _dbContext.GameRooms.Add(gameRoom);

            return gameRoom;
        }
    }
}
