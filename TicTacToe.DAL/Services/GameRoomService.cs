using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using TicTacToe.DAL.Models;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace TicTacToe.DAL.Services
{
    public class GameRoomService
    {
        GameDBContext _dbContext;

        public GameRoomService(GameDBContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<IList<GameRoom>> GetLobbyGames(string search = null, int pageSize = 20, int offset = 0, bool notFullOnly = false, bool withNoMinesOnly = false)
        {
            var result = _dbContext.GameRooms
                .Where(x => x.State == GameRoomState.New && !x.IsHidden);

            if(notFullOnly)
            {
                result = result.Where(x => x.GameRoomPlayers.Count < x.MaxPlayers);
            }

            if (withNoMinesOnly)
            {
                result = result.Where(x => x.MinesQuantity == 0);
            }

            if (search != null)
            {
                result = result.Where(x => x.RoomGuid.ToString().Contains(search));
            }

            return await result
                .Skip(0)
                .Take(pageSize)
                .ToListAsync();
        }

        public async Task<GameRoom> CreateGame(string creatorUserId, int maxPlayers = 2, int minesQuantity = 1, bool isHidden = false, string password = null)
        {
            var creatorPlayer = _dbContext.Users.SingleOrDefault(x => x.Id == creatorUserId);

            if (creatorPlayer == null) { throw new ArgumentException("There is no such user!"); }

            var gameRoom = new GameRoom();

            gameRoom.RoomGuid = Guid.NewGuid();
            gameRoom.State = GameRoomState.New;
            gameRoom.MaxPlayers = maxPlayers;
            gameRoom.Password = password;
            gameRoom.IsHidden = isHidden;
            gameRoom.MinesQuantity = minesQuantity;

            _dbContext.GameRooms.Add(gameRoom);
            
            await _dbContext.SaveChangesAsync();

            return gameRoom;
        }
    }
}
