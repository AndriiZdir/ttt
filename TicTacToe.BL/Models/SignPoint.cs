using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using TicTacToe.DAL.Enums;

namespace TicTacToe.BL.Models
{
    public struct SignPoint
    {
        private Point _position;

        public SignPoint(Point position, GameRoomPlayerSign sign)
        {
            _position = position;
            Sign = sign;
        }

        public SignPoint(int x, int y, GameRoomPlayerSign sign)
        {
            _position.X = x;
            _position.Y = y;
            Sign = sign;
        }


        public Point Position { get { return _position; } }
        public GameRoomPlayerSign Sign { get; set; }


        public Point SetPosition(int x, int y)
        {
            _position.X = x;
            _position.Y = y;

            return _position;
        }

        public bool IsEmpty => (Sign == GameRoomPlayerSign.Empty);
    }
}

