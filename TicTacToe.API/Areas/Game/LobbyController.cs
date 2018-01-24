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
        private readonly UserManager<ApplicationUser> _userManager;

        public LobbyController(GameRoomService gameRoomService, UserManager<ApplicationUser> userManager)
        {
            _gameRoomService = gameRoomService;
            _userManager = userManager;
        }

        [HttpGet]
        public object GameList()
        {
            var result = _gameRoomService.GetLobbyGames(notFullOnly: true);

            return ListResult(result);
        }

        [HttpGet("init")]
        [Authorize]
        public async Task<object> InitGame(LobbyInitGameModel model)
        {
            if (!ModelState.IsValid)
            {
                return ValidationResult();
            }

            var currentUser = await _userManager.Users.FirstAsync();

            var result = _gameRoomService.CreateGame(currentUser.Id, model.MaxPlayers, model.MinesQuantity, false, model.Password);

            return EntityResult(result);
        }

        [HttpGet("join")]
        [Authorize]
        public object JoinGame(LobbyInitGameModel model)
        {
            if (!ModelState.IsValid)
            {
                return ValidationResult();
            }

            var result = _gameRoomService.GetLobbyGames(notFullOnly: true);

            return ListResult(result);
        }
    }
}