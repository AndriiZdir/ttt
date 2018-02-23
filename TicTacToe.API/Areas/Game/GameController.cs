using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TicTacToe.API.Areas.Models;
using TicTacToe.API.Models;
using TicTacToe.BL;
using TicTacToe.DAL.Services;

namespace TicTacToe.API.Areas.Game
{
    [Route("api/game")]
    public class GameController : BaseGameController
    {
        private readonly GameRoomService _gameRoomService;
        private readonly GameManager _gameManager;

        public GameController(GameRoomService gameRoomService, GameManager gameManager, UserManager<ApplicationUser> userManager) : base(userManager)
        {
            _gameRoomService = gameRoomService;
            _gameManager = gameManager;
        }

        [HttpPost("start/{roomid}")]
        public async Task<object> StartGame(Guid RoomId)
        {
            var currentUser = await _userManager.GetUserAsync(User);

            var gameRoom = await _gameRoomService.StartGame(currentUser.Id, RoomId);

            var playerIds = gameRoom.GameRoomPlayers.Select(x => x.UserId);

            var gameField = _gameManager.StartNewGame(gameRoom.RoomGuid, playerIds);

            return StatusResult(200, $"Starting the game {RoomId}...");
        }

        [HttpGet("state/{roomid}")]
        public async Task<object> GameState(Guid RoomId)
        {
            var currentUser = await _userManager.GetUserAsync(User);

            var gameField = _gameManager[RoomId];

            //TODO: activate restriction ?
            //if (!gameField.Players.Any(x => x.Id == currentUser.Id)) { return Forbid(); }

            var gameState = GameCurrentStateModel.FromGameField(gameField);

            return EntityResult(gameState);
        }

        [HttpPost("point/{roomid}/{X};{Y}")]
        public async Task<object> SetPoint(Guid RoomId, int x, int y)
        {
            var currentUser = await _userManager.GetUserAsync(User);

            var point = _gameManager.SetPoint(RoomId, currentUser.Id, x, y);

            var gameField = _gameManager[RoomId];
            var gameState = GameCurrentStateModel.FromGameField(gameField);

            return EntityResult(gameState);
        }

        [HttpPost("mine/{roomid}/{X};{Y}")]
        public async Task<object> SetMine(Guid RoomId, int x, int y)
        {
            var currentUser = await _userManager.GetUserAsync(User);

            var point = _gameManager.SetPoint(RoomId, currentUser.Id, x, y);

            var gameField = _gameManager[RoomId];
            var gameState = GameCurrentStateModel.FromGameField(gameField);

            return EntityResult(gameState);
        }

        //TODO: GetGameSettings (game field size, number of players and their signs, mines quanity, frag limit, etc.)

        //TODO: Game Finishing

        //TODO: Player rating
    }
}
