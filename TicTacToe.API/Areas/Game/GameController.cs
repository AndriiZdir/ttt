using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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

        [HttpGet("start/{roomid}")]
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

            var gameField = _gameManager.GetGameField(RoomId);

            //TODO: activate restriction ?
            //if (!gameField.Players.Any(x => x.Id == currentUser.Id)) { return Forbid(); }

            return StatusResult(200, $"Starting the game {RoomId}...");
        }

        //TODO: GetGameSettings (game field size, number of players and their signs, mines quanity, frag limit, etc.)

        //TODO: GetGameState (points situation, combinations, players points, mines left?, etc.)

        //TODO: Put player point

        //TODO: Game Finishing

        //TODO: Player rating
    }
}
