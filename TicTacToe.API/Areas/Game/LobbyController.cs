using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TicTacToe.API.Areas.Models;
using TicTacToe.API.Models;
using TicTacToe.DAL.Models;
using TicTacToe.DAL.Services;

namespace TicTacToe.API.Areas.Game
{    
    [Route("api/lobby")] /* About attribute: https://docs.microsoft.com/ru-ru/aspnet/core/mvc/controllers/routing */
    public class LobbyController : BaseGameController
    {
        private readonly GameRoomService _gameRoomService;

        public LobbyController(GameRoomService gameRoomService, UserManager<ApplicationUser> userManager) : base(userManager)
        {
            _gameRoomService = gameRoomService;
        }        

        [HttpGet]
        [AllowAnonymous]
        public async Task<object> GameList(string search)
        {
            var result = await _gameRoomService
                .GetLobbyGames(search: search)
                .QuerySkipTake(0, 20)
                .Select(LobbyGameListItemModel.GetSelectExpression())
                .ToListAsync();

            return ListResult(result);
        }

        [HttpPost("init")]
        public async Task<object> InitGame(LobbyInitGameModel model)
        {
            if (!ModelState.IsValid)
            {
                return ValidationResult();
            }

            var currentUser = await _userManager.GetUserAsync(User);

            var result = await _gameRoomService.CreateGameRoom(currentUser.Id, model.MaxPlayers, model.MinesQuantity, false, model.Password);
            
            return Ok("Game room has been created");
        }

        [HttpPost("join/{roomid}")]
        public async Task<object> JoinGame(Guid roomId, string password = null)
        {
            var gameRoom = await _gameRoomService.FindRoomByGuidId(roomId);

            if (gameRoom == null) { return NotFoundResult("Room with such id not found"); }

            if (gameRoom.Password != password) { return Forbid("Wrong password"); }

            var currentUser = await _userManager.GetUserAsync(User);

            var gameRoomPlayer = await _gameRoomService.JoinPlayerToGameRoom(currentUser.Id, gameRoom.Id, password: password);

            return Ok("Player has been joined the room");
        }

        [HttpGet("details/{roomid}")]
        public async Task<object> GameDetails(Guid roomId, string password = null)
        {
            var gameRoom = await _gameRoomService.FindRoomByGuidId(roomId, true);

            if (gameRoom == null) { return NotFoundResult("Room with such id not found"); }

            if (gameRoom.Password != password) { return Forbid("Wrong password"); }

            var model = LobbyGameDetailsModel.LoadFromGameRoom(gameRoom);
            
            return EntityResult(model);
        }

        [HttpGet("kick/{roomid}/{playerid}")] //TODO: change to HttpPost
        public async Task<object> KickFromRoom(Guid roomId, string playerId)
        {
            var gameRoom = await _gameRoomService.FindRoomByGuidId(roomId);

            if (gameRoom == null) { return NotFoundResult("Room with such id not found"); }

            var currentUser = await _userManager.GetUserAsync(User);

            if (gameRoom.CreateUser != currentUser.Id 
                && gameRoom.State == GameRoomState.New) { return Forbid("You have no permission to do that."); }

            await _gameRoomService.KickPlayerFromGameRoom(playerId, gameRoom.Id);

            return Ok("Player has been kicked from the room by its creator");
        }

        [HttpPost("leave")]
        public async Task<object> LeaveGame()
        {
            //var gameRoom = await _gameRoomService.FindRoomByGuidId(RoomId);

            //if (gameRoom == null) { return NotFoundResult("Room with such id not found"); }

            var currentUser = await _userManager.GetUserAsync(User);

            await _gameRoomService.RemovePlayerFromGameRoom(currentUser.Id);

            return Ok("Player has been left all rooms");
        }

        [HttpPost("ready/{roomid}")]
        public async Task<object> SetAsReady(Guid RoomId)
        {
            //TODO: Autostart after last user is ready

            var gameRoom = await _gameRoomService.FindRoomByGuidId(RoomId);

            if (gameRoom == null) { return NotFoundResult("Room with such id not found"); }

            if (gameRoom.State == GameRoomState.Started
                || gameRoom.State == GameRoomState.Closed) { return BadRequest("Unable to set player status in this room."); }

            var currentUser = await _userManager.GetUserAsync(User);

            var gameRoomPlayer = await _gameRoomService.ChangePlayerState(currentUser.Id, gameRoom.Id, DAL.Enums.GameRoomPlayerState.Ready);

            return Ok("Player is Ready");
        }

    }
}