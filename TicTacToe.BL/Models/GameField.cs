using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using TicTacToe.DAL.Enums;

namespace TicTacToe.BL.Models
{
    public class GameField
    {
        const int FIELD_SIZE = 2000;        
        const int FIELD_POINT_TANSFORM_X = (FIELD_SIZE / 2);
        const int FIELD_POINT_TANSFORM_Y = FIELD_POINT_TANSFORM_X;
        const int MAX_BOUND_INCREASE = 10;

        private SignPoint[,] _field;
        private Guid _gameId;
        private Rectangle _gameBounds;
        private List<Combination> _gameCombinations;

        public GameField()
        {
            _field = new SignPoint[FIELD_SIZE, FIELD_SIZE];
            _gameId = Guid.NewGuid();
            _gameBounds = Rectangle.Empty;
            _gameCombinations = new List<Combination>();
        }

        public Guid GameId { get { return _gameId; } }

        public Rectangle Bounds { get { return _gameBounds; } }

        public void SetPointSign(int x, int y, GameRoomPlayerSign sign)
        {
            var signPoint = GetSignPointByCoords(x, y);

            if (!signPoint.IsEmpty)
            {
                throw new Exception("This point is not empty!");
            }

            var point = SetSignPointByCoords(x, y, sign);
            var position = point.Position;

            if (!_gameBounds.Contains(position))
            {
                int left = position.X < _gameBounds.Left ? position.X : _gameBounds.Left;
                int right = position.X > _gameBounds.Right ? position.X : _gameBounds.Right;
                int top = position.Y < _gameBounds.Top ? position.Y : _gameBounds.Top;
                int bottom = position.Y > _gameBounds.Bottom ? position.Y : _gameBounds.Bottom;

                _gameBounds = Rectangle.FromLTRB(left, top, right, bottom);
            }
        }

        #region Utilities

        private SignPoint GetSignPointByCoords(int x, int y)
        {
            return _field[x + FIELD_POINT_TANSFORM_X, y + FIELD_POINT_TANSFORM_Y];
        }

        private SignPoint SetSignPointByCoords(int x, int y, GameRoomPlayerSign sign)
        {
            ref SignPoint point = ref _field[x + FIELD_POINT_TANSFORM_X, y + FIELD_POINT_TANSFORM_Y];

            point.Sign = sign;
            point.SetPosition(x, y);

            return point;
        }

        #endregion
    }
}
