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

        private SignPoint[,] _field;
        private Guid _gameId;
        private Rectangle _gameBounds;
        private readonly Rectangle _maxGameFieldBounds;
        private List<SignPoint> _gamePoints;
        private List<Combination> _gameCombinations;

        protected GameFieldState _gameState;

        private List<Player> _gamePlayers;
        protected Player _currentTurnPlayer;        
        
        public GameField(int fieldSize)
        {
            _fieldSize = fieldSize;
            field_point_tansform_x = _fieldSize / 2;
            field_point_tansform_y = field_point_tansform_x;

            _field = new SignPoint[_fieldSize, _fieldSize];
            _gameId = Guid.NewGuid();
            _gameBounds = Rectangle.Empty;
            _maxGameFieldBounds = Rectangle.FromLTRB(2 - field_point_tansform_x, 2 - field_point_tansform_y, field_point_tansform_x - 2, field_point_tansform_y - 2);
            _gamePoints = new List<SignPoint>();
            _gameCombinations = new List<Combination>();
            _gamePlayers = new List<Player>();
            _currentTurnPlayer = null;
            _gameState = GameFieldState.New;            
        }

        public Guid GameId { get { return _gameId; } }

        public GameFieldState State { get { return _gameState; } }

        public Rectangle Bounds { get { return _gameBounds; } }

        public List<SignPoint> Points { get { return _gamePoints; } }

        public List<Combination> Combinations { get { return _gameCombinations; } }

        public List<Player> Players { get { return _gamePlayers; } }

        public Player CurrentTurnPlayer { get { return _currentTurnPlayer; } }

        public Player AddPlayerToField(Guid playerId)
        {
            if (_gameState != GameFieldState.New && _gameState != GameFieldState.Ready) { throw new Exception("Game already started"); }
            if (_gamePlayers.Count >= MAX_PLAYERS) { throw new Exception("Maximum players count is " + MAX_PLAYERS); }

            var player = new Player(playerId);
            _gamePlayers.Add(player);

            if (_gamePlayers.Count >= 2 && _currentTurnPlayer == null)
            {
                UpdateNextTurnPlayer();
                _gameState = GameFieldState.Ready;
            }

            return player;
        }

        public SignPoint SetPointSign(int x, int y)
        {
            if (_gameState != GameFieldState.Ready && _gameState != GameFieldState.Started)
            {
                throw new InvalidOperationException("Game is not going.");
            }

            CheckBoundsOutOfRange(x, y);

            var point = GetPointByCoords(x, y);

            if (point == null)
            {
                point = SetPointByCoords(x, y, _currentTurnPlayer, SignPointType.Sign);

                if (_gameState == GameFieldState.Ready)
                {
                    _gameState = GameFieldState.Started;
                }

                UpdateNextTurnPlayer();
            }
            else if (point != null && point.PointType == SignPointType.MineNew)
            {
                point.ExplodeMine(_currentTurnPlayer);
                _currentTurnPlayer.SkipNextTurn = true;

                UpdateNextTurnPlayer();
            }
            else
            {
                throw new Exception("This point is not empty!");
            }

            UpdateFieldBounds(point.Position);

            var neighbourPoints = GetNeighbourPoints(point);

            foreach (var neighbourPoint in neighbourPoints)
            {
                var direction = neighbourPoint ^ point;
                var pCombination = point.GetPointCombinationForDirection(direction);
                var nCombination = neighbourPoint.GetPointCombinationForDirection(direction);

                if (nCombination != null) //neighbour point is in combination
                {
                    if (pCombination == null)  //new added point is NOT in combination
                    {
                        if (!nCombination.IsReadOnly)
                        {
                            point.AddToCombination(nCombination);
                        }
                    }
                    else //new added point is in combination
                    {
                        foreach (var pcPoint in pCombination.Points)
                        {
                            //Combination merge
                            pcPoint.AddToCombination(nCombination);

                            pCombination.State = CombinationState.Closed;
                            _gameCombinations.Remove(pCombination);
                        }
                    }
                }
                else //neighbour point is NOT in combination
                {
                    if (pCombination != null) //new added point is in combination
                    {
                        if (!pCombination.IsReadOnly)
                        {
                            neighbourPoint.AddToCombination(pCombination);
                        }
                    }
                    else //new added point is NOT in combination
                    {
                        Combination newCombination = new Combination(direction); //TODO: add row size setting

                        _gameCombinations.Add(newCombination);

                        point.AddToCombination(newCombination);
                        neighbourPoint.AddToCombination(newCombination);
                    }
                }
            }


            return point;
        }

        public void SetMine(int x, int y)
        {
            CheckBoundsOutOfRange(x, y);

            var point = GetPointByCoords(x, y);

            if (point != null && point.PointType == SignPointType.MineNew)
            {
                point.ExplodeMine(_currentTurnPlayer);
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

        private int _nextTurnPlayerIndex = 0;
        protected Player UpdateNextTurnPlayer()
        {
            Player next = null;

            do
            {
                var player = _gamePlayers[_nextTurnPlayerIndex];

                if (player.SkipNextTurn)
                {
                    player.SkipNextTurn = false;
                }
                else
                {
                    next = player;
                }

                _nextTurnPlayerIndex++;

                if (_nextTurnPlayerIndex >= _gamePlayers.Count) { _nextTurnPlayerIndex = 0; }

            } while (next == null);

            _currentTurnPlayer = next;

            return next;
        }


        protected SignPoint GetPointByCoords(int x, int y)
        {
            return _field[x + field_point_tansform_x, y + field_point_tansform_y];
        }

        protected SignPoint SetPointByCoords(int x, int y, Player player, SignPointType pointType)
        {
            var point = new SignPoint(player, x, y, pointType);

            _gamePoints.Add(point);

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

                    if (nPoint != null && (!onlySameSign || (nPoint.PointType == point.PointType && nPoint.Player.Id == point.Player.Id)))
                    {
                        yield return nPoint;
                    }
                }
            }
            
        }

        public void CompleteTheGame()
        {
            _gameState = GameFieldState.Completed;
            
            var maxPoints = _gamePlayers.Max(x => x.Points);

            foreach (var player in _gamePlayers)
            {
                if (player.Points == maxPoints)
                {
                    player.State = PlayerState.Winner;
                }
                else
                {
                    player.State = PlayerState.Loser;
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
