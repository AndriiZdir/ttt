using System;
using System.Collections.Generic;
using System.Text;

namespace TicTacToe.BL.Models
{
    public class Player
    {
        public Player(Guid playerId)
        {
            Id = playerId;
            _mines = 1;
            SkipNextTurn = false;
        }

        public Guid Id { get; private set; }

        private int _mines;
        public int Mines => _mines;

        private int _points;
        public int Points => _points;

        public bool SkipNextTurn { get; set; }

        public int AddPoints(int count = 1)
        {
            _points += count;

            return _points;
        }

        public int UseMine()
        {
            return --_mines;
        }
    }
}
