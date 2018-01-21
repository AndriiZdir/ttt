using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;

namespace TicTacToe.BL.Models
{
    public class GameField
    {
        #region readonlies
        private readonly int _fieldSize;
        private readonly int field_point_tansform_x;
        private readonly int field_point_tansform_y;
        private readonly int max_bound_increase = 10;
        private readonly int MAX_PLAYERS = 3;
        #endregion

        private int pointCount = 0;
        private SignPoint[,] _field;
        private Guid _gameId;
        private Rectangle _gameBounds;
        private readonly Rectangle _maxGameFieldBounds;
        private List<Combination> _gameCombinations;

        private GameFieldState _gameState;

        private List<Player> _gamePlayers;
        private Player _currentTurnPlayer;        
        
        public GameField(int fieldSize)
        {
            _fieldSize = fieldSize;
            field_point_tansform_x = _fieldSize / 2;
            field_point_tansform_y = field_point_tansform_x;

            _field = new SignPoint[_fieldSize, _fieldSize];
            _gameId = Guid.NewGuid();
            _gameBounds = Rectangle.Empty;
            _maxGameFieldBounds = Rectangle.FromLTRB(2 - field_point_tansform_x, 2 - field_point_tansform_y, field_point_tansform_x - 2, field_point_tansform_y - 2);
            _gameCombinations = new List<Combination>();
            _gameState = GameFieldState.New;
            _currentTurnPlayer = null;
        }

        public Guid GameId { get { return _gameId; } }

        public Rectangle Bounds { get { return _gameBounds; } }

        public List<Combination> Combinations { get { return _gameCombinations; } }

        public List<Player> Players { get { return _gamePlayers; } }

        public void AddPlayerToField(Guid playerId)
        {
            if (_gameState != GameFieldState.New && _gameState != GameFieldState.Ready) { throw new Exception("Game already started"); }
            if (_gamePlayers.Count >= MAX_PLAYERS) { throw new Exception("Maximum players count is " + MAX_PLAYERS); }

            _gamePlayers.Add(new Player(playerId));

            if (_gamePlayers.Count >= 2 && _currentTurnPlayer == null)
            {
                UpdateNextTurnPlayer();
                _gameState = GameFieldState.Ready;
            }
        }

        public void SetPointSign(int x, int y)
        {
            CheckBoundsOutOfRange(x, y);

            var point = GetPointByCoords(x, y);

            if (point != null && point.PointType == SignPointType.MineNew)
            {
                point.ExplodeMine(_currentTurnPlayer.Id);
                _currentTurnPlayer.SkipNextTurn = true;
            }
            else if (point == null || point.PointType == SignPointType.Empty)
            {
                point = SetPointByCoords(x, y, _currentTurnPlayer, SignPointType.Sign);
            }
            else
            {
                throw new Exception("This point is not empty!");
            }

            UpdateFieldBounds(point.Position);

            List<SignPoint> neighbourPoints = GetNeighbourPoints(point).ToList();

            //foreach (var neighbourPoint in neighbourPoints)
            //{
            //    var direction = point.GetDirectionWith(neighbourPoint);

            //    var combinations = GetCombinationsWithPoint(neighbourPoint).ToList();

            //    if (combinations.Count > 0)
            //    {
            //        foreach (var combination in combinations)
            //        {
            //            if (combination.Direction != direction)
            //            {
            //                var newCombination = new Combination(direction);

            //                newCombination.AddPoint(point);
            //                newCombination.AddPoint(neighbourPoint);

            //                _gameCombinations.Add(newCombination);
            //            }
            //            else
            //            {
            //                combination.AddPoint(point);
            //            }
            //        }
            //    }
            //    else
            //    {
            //        var combination = new Combination(direction);

            //        combination.AddPoint(point);
            //        combination.AddPoint(neighbourPoint);

            //        _gameCombinations.Add(combination);
            //    }
            //}

            UpdateNextTurnPlayer();
        }

        public void SetMine(int x, int y)
        {
            CheckBoundsOutOfRange(x, y);

            var point = GetPointByCoords(x, y);

            if (point != null && point.PointType == SignPointType.MineNew)
            {
                point.ExplodeMine(_currentTurnPlayer.Id);
                _currentTurnPlayer.SkipNextTurn = true;
            }
            else if (point == null || point.PointType == SignPointType.Empty)
            {
                point = SetPointByCoords(x, y, _currentTurnPlayer, SignPointType.MineNew);
            }
            else
            {
                throw new Exception("This point is not empty!");
            }

            UpdateFieldBounds(point.Position);
        }

        #region Utilities

        protected Player GetPlayerById(Guid playerId)
        {
            return _gamePlayers.Find(x => x.Id == playerId);
        }

        private int _nextTurnPlayerIndex = 0;
        protected Player UpdateNextTurnPlayer()
        {
            Player next = null;

            do
            {
                if (next != null)
                {
                    next.SkipNextTurn = false;
                }

                next = _gamePlayers[_nextTurnPlayerIndex];

                _nextTurnPlayerIndex++;

                if (_gamePlayers.Count >= _nextTurnPlayerIndex) { _nextTurnPlayerIndex = 0; }

            } while (next.SkipNextTurn);

            _currentTurnPlayer = next;

            return next;
        }


        protected SignPoint GetPointByCoords(int x, int y)
        {
            return _field[x + field_point_tansform_x, y + field_point_tansform_y];
        }

        protected SignPoint SetPointByCoords(int x, int y, Player player, SignPointType pointType)
        {
            //SignPoint point = GetPointByCoords(x, y);

            //if (point == null) { pointCount++; }

            var point = new SignPoint(player.Id, x, y, pointType);

            _field[x + field_point_tansform_x, y + field_point_tansform_y] = point;            

            return point;
        }


        protected void UpdateFieldBounds(Point position)
        {
            if (_gameBounds.Contains(position)) { return; }

            int left = position.X < _gameBounds.Left ? position.X : _gameBounds.Left;
            int right = position.X > _gameBounds.Right ? position.X : _gameBounds.Right;
            int top = position.Y < _gameBounds.Top ? position.Y : _gameBounds.Top;
            int bottom = position.Y > _gameBounds.Bottom ? position.Y : _gameBounds.Bottom;

            if (_gameBounds.Left != left && (_gameBounds.Left - left) > max_bound_increase) { throw new ArgumentOutOfRangeException(nameof(position), "Large bound increase"); }
            if (_gameBounds.Right != right && (right - _gameBounds.Right) > max_bound_increase) { throw new ArgumentOutOfRangeException(nameof(position), "Large bound increase"); }
            if (_gameBounds.Top != top && (_gameBounds.Top - top) > max_bound_increase) { throw new ArgumentOutOfRangeException(nameof(position), "Large bound increase"); }
            if (_gameBounds.Bottom != bottom && (bottom - _gameBounds.Bottom) > max_bound_increase) { throw new ArgumentOutOfRangeException(nameof(position), "Large bound increase"); }

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

        protected void GetSuitableCombination(SignPoint point1, SignPoint point2)
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

                    var nPoint = GetPointByCoords(x + dx, y + dy);

                    if (!nPoint.IsEmpty && (!onlySameSign || nPoint.PointType == point.PointType))
                    {
                        yield return nPoint;
                    }
                }
            }
            
        }

        protected IEnumerable<Combination> GetCombinationsWithPoint(SignPoint point)
        {
            foreach(var gameCombination in _gameCombinations)
            {
                if (gameCombination.Points.Contains(point))
                {
                    yield return gameCombination;
                    continue;
                }
            }
        }

        #endregion
    }

    public enum GameFieldState
    {
        New,
        Ready,
        Started,
        Completed
    }
}
