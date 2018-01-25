﻿using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using TicTacToe.DAL.Models;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TicTacToe.DAL.Enums;

namespace TicTacToe.DAL.Services
{
    public class GameRoomService
    {
        GameDBContext _dbContext;

        public GameRoomService(GameDBContext dbContext)
        {
            _dbContext = dbContext;
        }        

        public IQueryable<GameRoom> GetLobbyGames(string search = null, int pageSize = 20, int offset = 0, bool notFullOnly = false, bool withNoMinesOnly = false)
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

            return result;
        }

        public Task<GameRoom> FindRoomByGuidId(Guid roomId)
        {
            return _dbContext.GameRooms.SingleOrDefaultAsync(x => x.RoomGuid == roomId);
        }

        public async Task<GameRoom> CreateGameRoom(string creatorUserId, int maxPlayers = 2, int minesQuantity = 1, bool isHidden = false, string password = null)
        {
            var creatorPlayer = await _dbContext.Users.SingleOrDefaultAsync(x => x.Id == creatorUserId);

            if (creatorPlayer == null) { throw new ArgumentException("There is no such user!"); }

            var gameRoom = new GameRoom();

            gameRoom.RoomGuid = Guid.NewGuid();
            gameRoom.State = GameRoomState.New;
            gameRoom.MaxPlayers = maxPlayers;
            gameRoom.Password = password;
            gameRoom.IsHidden = isHidden;
            gameRoom.MinesQuantity = minesQuantity;

            gameRoom.CreateUser = creatorUserId;
            gameRoom.LastEditUser = creatorUserId;
            gameRoom.CreateDate = DateTime.UtcNow;
            gameRoom.LastEditDate = DateTime.UtcNow;

            _dbContext.GameRooms.Add(gameRoom);

            gameRoom.GameRoomPlayers.Add(new GameRoomPlayer {
                UserId = creatorUserId,                
                PlayerSign = GameRoomPlayerSign.Cross,
                PlayerState = GameRoomPlayerState.Waiting,
                PlayerType = GameRoomPlayerType.Creator });

            await _dbContext.SaveChangesAsync();

            return gameRoom;
        }

        public async Task<GameRoomPlayer> JoinPlayerToGameRoom(string playerId, int gameId, GameRoomPlayerSign? sign = null, string password = null)
        {
            var gameRoom = await _dbContext.GameRooms
                .Include(x => x.GameRoomPlayers)
                .SingleOrDefaultAsync(x => x.Id == gameId);

            if (gameRoom.State == GameRoomState.Started || gameRoom.State == GameRoomState.Closed)
            {
                throw new Exception("This game is started already.");
            }

            if (gameRoom.GameRoomPlayers.Count >= gameRoom.MaxPlayers)
            {
                throw new Exception("The maximum number of players in this room has been reached.");
            }

            if (gameRoom.Password != password)
            {
                throw new Exception("Password is invalid");
            }

            #region Check participation in other games            
            var playerActiveGames = await _dbContext
                .GameRoomPlayers
                .Include(x => x.GameRoom)
                .Where(x => x.UserId == playerId
                            && x.GameRoomId != gameRoom.Id
                            && (x.PlayerState == GameRoomPlayerState.Ready || x.PlayerState == GameRoomPlayerState.Waiting)
                            && (x.GameRoom.State == GameRoomState.Started || x.GameRoom.State == GameRoomState.New))
                .ToListAsync();

            foreach (var playerActiveGame in playerActiveGames)
            {
                if (playerActiveGame.GameRoom.State == GameRoomState.New)
                {
                    if (playerActiveGame.PlayerType == GameRoomPlayerType.Creator)
                    {
                        _dbContext.GameRooms.Remove(playerActiveGame.GameRoom);
                    }
                    else
                    {
                        _dbContext.GameRoomPlayers.Remove(playerActiveGame);
                    }                    
                }
                else if (playerActiveGame.GameRoom.State == GameRoomState.Started)
                {
                    playerActiveGame.PlayerState = GameRoomPlayerState.Left;
                    //TODO: game manager player disconnection
                }
            }
            #endregion

            await _dbContext.SaveChangesAsync();

            var entity = new GameRoomPlayer();

            entity.UserId = playerId;
            entity.GameRoomId = gameId;
            entity.PlayerSign = sign ?? GetFreeSigns(gameRoom.GameRoomPlayers).First();
            entity.PlayerState = GameRoomPlayerState.Waiting;
            entity.PlayerType = GameRoomPlayerType.Regular;

            _dbContext.GameRoomPlayers.Add(entity);

            await _dbContext.SaveChangesAsync();

            return entity;
        }

        public async Task<GameRoomPlayer> RemovePlayerFromGameRoom(string playerId, int gameId)
        {
            var gameRoomPlayer = await _dbContext.GameRoomPlayers
                .Include(x => x.GameRoom)
                .SingleOrDefaultAsync(x => x.UserId == playerId && x.GameRoomId == gameId);

            if (gameRoomPlayer.GameRoom.State == GameRoomState.New)
            {
                if (gameRoomPlayer.PlayerType == GameRoomPlayerType.Creator)
                {
                    _dbContext.GameRooms.Remove(gameRoomPlayer.GameRoom);
                }
                else
                {
                    _dbContext.GameRoomPlayers.Remove(gameRoomPlayer);
                }                
            }
            else
            {
                gameRoomPlayer.PlayerState = GameRoomPlayerState.Left;
                //TODO: game manager player disconnection
            }

            await _dbContext.SaveChangesAsync();

            return gameRoomPlayer;
        }

        public async Task<GameRoomPlayer> ChangePlayerSign(string playerId, int gameId, GameRoomPlayerSign sign)
        {
            var gameRoomPlayers = await _dbContext.GameRoomPlayers.Where(x => x.GameRoomId == gameId).ToListAsync();

            var gameRoomPlayer = gameRoomPlayers.Find(x => x.UserId == playerId);

            if (gameRoomPlayer == null)
            {
                throw new Exception("There is no such user in this game room!");
            }

            if (gameRoomPlayers.Any(x => x.PlayerSign == sign && x.UserId != playerId))
            {
                throw new Exception("There is user with same sign in current room! Please, choose the other sign.");
            }

            gameRoomPlayer.PlayerSign = sign;

            await _dbContext.SaveChangesAsync();

            return gameRoomPlayer;
        }

        #region Utilities
        private IEnumerable<GameRoomPlayerSign> GetFreeSigns(ICollection<GameRoomPlayer> roomPlayers)
        {
            if (roomPlayers == null
                || roomPlayers.Count == 0
                || !roomPlayers.Any(x => x.PlayerSign == GameRoomPlayerSign.Cross)) { yield return GameRoomPlayerSign.Cross; }
            if (!roomPlayers.Any(x => x.PlayerSign == GameRoomPlayerSign.Zero)) { yield return GameRoomPlayerSign.Zero; }
            if (!roomPlayers.Any(x => x.PlayerSign == GameRoomPlayerSign.Zed)) { yield return GameRoomPlayerSign.Zed; }
            if (!roomPlayers.Any(x => x.PlayerSign == GameRoomPlayerSign.Aitch)) { yield return GameRoomPlayerSign.Aitch; }
            if (!roomPlayers.Any(x => x.PlayerSign == GameRoomPlayerSign.Wy)) { yield return GameRoomPlayerSign.Wy; }
        }
        #endregion

    }
}
