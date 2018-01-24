using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace TicTacToe.API.Areas.Models
{
    public class LobbyInitGameModel
    {
        public LobbyInitGameModel()
        {
            FragLimit = int.MaxValue;
            MaxPlayers = 2;
            MinesQuantity = 1;
        }

        [Range(0, int.MaxValue)]
        public int FragLimit { get; set; }
        [Range(0, 5)]
        public int MaxPlayers { get; set; }
        public string Password { get; set; }
        [Range(0, 20)]
        public int MinesQuantity { get; set; }
        //public bool IsHidden { get; set; }
    }
}
