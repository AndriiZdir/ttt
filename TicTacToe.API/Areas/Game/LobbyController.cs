﻿using System;
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
        private readonly UserManager<ApplicationUser> _userManager;

        public LobbyController(GameRoomService gameRoomService, UserManager<ApplicationUser> userManager)
        {
            _gameRoomService = gameRoomService;
            _userManager = userManager;
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

        [HttpGet("init")]        
        public async Task<object> InitGame(LobbyInitGameModel model)
        {
            if (!ModelState.IsValid)
            {
                return ValidationResult();
            }

            var currentUser = await _userManager.GetUserAsync(User);

            var result = await _gameRoomService.CreateGameRoom(currentUser.Id, model.MaxPlayers, model.MinesQuantity, false, model.Password);
            
            return StatusResult(200, "Game room has been created");
        }

        [HttpGet("join/{roomid}")]
        public async Task<object> JoinGame(Guid RoomId, string Password = null)
        {
            var gameRoom = await _gameRoomService.FindRoomByGuidId(RoomId);

            if (gameRoom == null) { return NotFoundResult("Room with such id not found"); }

            var currentUser = await _userManager.GetUserAsync(User);

            var gameRoomPlayer = await _gameRoomService.JoinPlayerToGameRoom(currentUser.Id, gameRoom.Id, password: Password);

            return StatusResult(200, $"Player has been joined the room {RoomId}.");
        }

        [HttpGet("leave")]
        public async Task<object> LeaveGame()
        {
            //var gameRoom = await _gameRoomService.FindRoomByGuidId(RoomId);

            //if (gameRoom == null) { return NotFoundResult("Room with such id not found"); }

            var currentUser = await _userManager.GetUserAsync(User);

            await _gameRoomService.RemovePlayerFromGameRoom(currentUser.Id);

            return StatusResult(200, $"Player has been left all rooms.");
        }

        [HttpGet("ready/{roomid}")]
        public async Task<object> SetAsReady(Guid RoomId)
        {
            var gameRoom = await _gameRoomService.FindRoomByGuidId(RoomId);

            if (gameRoom == null) { return NotFoundResult("Room with such id not found"); }

            if (gameRoom.State == GameRoomState.Started
                || gameRoom.State == GameRoomState.Closed) { return StatusResult(400, "Unable to set player status in this room."); }

            var currentUser = await _userManager.GetUserAsync(User);

            var gameRoomPlayer = await _gameRoomService.ChangePlayerState(currentUser.Id, gameRoom.Id, DAL.Enums.GameRoomPlayerState.Ready);

            return StatusResult(200, $"Player is Ready.");
        }

        [HttpGet("start/{roomid}")]
        public async Task<object> StartGame(Guid RoomId)
        {
            var currentUser = await _userManager.GetUserAsync(User);

            await _gameRoomService.StartGame(currentUser.Id, RoomId);

            return StatusResult(200, $"Starting the game...");
        }

        //[HttpGet("details/{roomid}")]
        //public async Task<object> GameDetails(Guid RoomId)
        //{
        //    var result = await _gameRoomService.GetLobbyGames(notFullOnly: true).ToListAsync();

        //    return ListResult(result);
        //}
    }
}