using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace TicTacToe.DAL.Models
{
    public class GameRoom
    {
        public int Id { get; set; }
        public Guid RoomGuid { get; set; }
        public GameRoomState State { get; set; }
        public int FragLimit { get; set; }
        public int MaxPlayers { get; set; }
        public string Password { get; set; }
        public bool IsHidden { get; set; }

        [InverseProperty("GameRoom")]
        public virtual ICollection<GameRoomPlayer> GameRoomPlayers { get; set; }
    }

    public enum GameRoomState
    {
        New,
        Started,
        Closed
    }
}
