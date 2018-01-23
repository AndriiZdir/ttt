using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using TicTacToe.DAL.Enums;

namespace TicTacToe.DAL.Models
{
    public class GameRoomPlayer
    {
        public int GameRoomId { get; set; }
        public string UserId { get; set; }

        public GameRoomPlayerType PlayerType { get; set; }
        public GameRoomPlayerState PlayerState { get; set; }
        public GameRoomPlayerSign PlayerSign { get; set; }

        [ForeignKey("UserId")]
        public virtual Player Player { get; set; }

        [ForeignKey("GameRoomId")]
        [InverseProperty("GameRoomPlayers")]
        public virtual GameRoom GameRoom { get; set; }
    }
}
