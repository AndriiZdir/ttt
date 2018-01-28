using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using TicTacToe.BL.Models;

namespace TicTacToe.API.Areas.Models
{
    public class GameSettingsModel
    {
        public Guid GameId { get; set; }

        public Rectangle GlobalBounds { get; set; }

        public List<Player> Players { get; set; }

        public int FragLimit { get; set; }

        public int MaxPlayers { get; set; }

        public int MinesCount { get; set; }

        public GameFieldState GameState { get; set; }
    }
}
