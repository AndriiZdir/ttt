using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TicTacToe.DAL.Services;

namespace TicTacToe.API.Areas.Game
{    
    [Route("api/lobby")] /* About attribute: https://docs.microsoft.com/ru-ru/aspnet/core/mvc/controllers/routing */
    public class LobbyController : BaseGameController
    {
        GameRoomService _gameRoomService;

        public LobbyController(GameRoomService gameRoomService)
        {
            _gameRoomService = gameRoomService;
        }

        [HttpGet]
        public object GameList()
        {
            var result = _gameRoomService.GetLobbyGames(notFullOnly: true);

            return ListResult(result);
        }
    }
}