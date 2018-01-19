using System;
using System.Collections.Generic;
using System.Text;
using TicTacToe.DAL.Enums;

namespace TicTacToe.DAL.Models
{
    public class GameRoomPlayer
    {
        public int GameRoomId { get; set; }
        public string PlayerId { get; set; }

        public GameRoomPlayerType PlayerType { get; set; }
        public GameRoomPlayerState PlayerState { get; set; }
        public GameRoomPlayerSign PlayerSign { get; set; }
    }
}
