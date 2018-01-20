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
        #region Constants
        const int FIELD_SIZE = 2000;
        const int FIELD_POINT_TANSFORM_X = (FIELD_SIZE / 2);
        const int FIELD_POINT_TANSFORM_Y = FIELD_POINT_TANSFORM_X;
        const int MAX_BOUND_INCREASE = 10;
        #endregion

        private SignPoint[,] _field;
        private Guid _gameId;
        private Rectangle _gameBounds;
        private readonly Rectangle _maxGameFieldBounds;
        private List<Combination> _gameCombinations;        

        public GameField()
        {
            _field = new SignPoint[FIELD_SIZE, FIELD_SIZE];
            _gameId = Guid.NewGuid();
            _gameBounds = Rectangle.Empty;
            _maxGameFieldBounds = Rectangle.FromLTRB(2 - FIELD_POINT_TANSFORM_X, 2 - FIELD_POINT_TANSFORM_Y, FIELD_POINT_TANSFORM_X - 2, FIELD_POINT_TANSFORM_Y - 2);
            _gameCombinations = new List<Combination>();
        }

        public Guid GameId { get { return _gameId; } }

        public Rectangle Bounds { get { return _gameBounds; } }

        public void SetPointSign(int x, int y, GameRoomPlayerSign sign)
        {
            if (sign == GameRoomPlayerSign.Empty)
            {
                throw new ArgumentException("Setting sign cannot be Empty", nameof(sign));
            }

            CheckBoundsOutOfRange(x, y);

            var signPoint = GetSignPointByCoords(x, y);

            if (!signPoint.IsEmpty)
            {
                throw new Exception("This point is not empty!");
            }            

            var point = SetSignPointByCoords(x, y, sign);

            UpdateFieldBounds(point.Position);

            List<SignPoint> neighbourPoints = GetNeighbourPoints(point).ToList();


        }

        #region Utilities

        protected SignPoint GetSignPointByCoords(int x, int y)
        {
            return _field[x + FIELD_POINT_TANSFORM_X, y + FIELD_POINT_TANSFORM_Y];
        }

        protected SignPoint SetSignPointByCoords(int x, int y, GameRoomPlayerSign sign)
        {
            ref SignPoint point = ref _field[x + FIELD_POINT_TANSFORM_X, y + FIELD_POINT_TANSFORM_Y];

            point.Sign = sign;
            point.SetPosition(x, y);

            return point;
        }

        protected void UpdateFieldBounds(Point position)
        {
            if (_gameBounds.Contains(position)) { return; }

            int left = position.X < _gameBounds.Left ? position.X : _gameBounds.Left;
            int right = position.X > _gameBounds.Right ? position.X : _gameBounds.Right;
            int top = position.Y < _gameBounds.Top ? position.Y : _gameBounds.Top;
            int bottom = position.Y > _gameBounds.Bottom ? position.Y : _gameBounds.Bottom;

            if (_gameBounds.Left != left && (_gameBounds.Left - left) > MAX_BOUND_INCREASE) { throw new ArgumentOutOfRangeException(nameof(position), "Large bound increase"); }
            if (_gameBounds.Right != right && (right - _gameBounds.Right) > MAX_BOUND_INCREASE) { throw new ArgumentOutOfRangeException(nameof(position), "Large bound increase"); }
            if (_gameBounds.Top != top && (_gameBounds.Top - top) > MAX_BOUND_INCREASE) { throw new ArgumentOutOfRangeException(nameof(position), "Large bound increase"); }
            if (_gameBounds.Bottom != bottom && (bottom - _gameBounds.Bottom) > MAX_BOUND_INCREASE) { throw new ArgumentOutOfRangeException(nameof(position), "Large bound increase"); }

            _gameBounds = Rectangle.FromLTRB(left, top, right, bottom);
        }

        protected void CheckBoundsOutOfRange(int x, int y)
        {
            CheckBoundsOutOfRange(new Point(x, y));
        }

        protected void CheckBoundsOutOfRange(Point position)
        {
            if (!_maxGameFieldBounds.Contains(position))
            {
                throw new ArgumentOutOfRangeException(nameof(position), position, "Out of game field bounds!");
            }
        }

        protected void GetSuitableCombination(SignPoint point)
        {

        }

        protected IEnumerable<SignPoint> GetNeighbourPoints(SignPoint point, bool onlySameSign = true)
        {
            int x = point.Position.X,
                y = point.Position.Y;

            for (int dx = -1; dx <= 1; dx++)
            {
                for (int dy = -1; dy <= 1; dy++)
                {
                    if (dx == 0 && dy == 0) { continue; }

                    var nPoint = GetSignPointByCoords(x + dx, y + dy);

                    if (!nPoint.IsEmpty && (!onlySameSign || nPoint.Sign == point.Sign))
                    {
                        yield return nPoint;
                    }
                }
            }
            
        }

        #endregion
    }
}
