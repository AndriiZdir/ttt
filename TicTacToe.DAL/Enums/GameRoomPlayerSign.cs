using System;
using System.Collections.Generic;
using System.Text;

namespace TicTacToe.DAL.Enums
{
    public enum GameRoomPlayerSign : byte
    {
        Empty,
        Cross,
        Zero,
        Poops,

        Mine = 0b11111111
    }
}
