using System;
using System.Collections.Generic;
using System.Text;

namespace TicTacToe.BL.Models
{
    public class Player
    {
        public Player(string playerId)
        {
            Id = playerId;
            _mines = 1;
            SkipNextTurn = false;
            State = PlayerState.InGame;
        }

        public string Id { get; private set; }

        public PlayerState State { get; set; }

        private int _mines;
        public int Mines => _mines;

        private int _points;
        public int Points => _points;

        public bool SkipNextTurn { get; set; }

        public int AddPoints(int count)
        {
            _points += count;

            return _points;
        }

        public int GiveMine()
        {
            return ++_mines;
        }

        public int UseMine()
        {
            return --_mines;
        }
    }

    public enum PlayerState
    {
        InGame,
        Winner,
        Loser
    }

}
