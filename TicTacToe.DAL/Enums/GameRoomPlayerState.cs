using System;
using System.Collections.Generic;
using System.Text;

namespace TicTacToe.DAL.Enums
{
    public enum GameRoomPlayerState
    {
        Waiting,
        Ready,
        /// <summary>
        ///If user joins other game, he will be forcible disconnected from the current one.
        /// </summary>
        Left
    }
}
