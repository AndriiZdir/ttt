using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;

namespace TicTacToe.BL.Models
{
    public class SignPoint
    {
        private Point _position;
        private Guid _playerId;
        private SignPointType _pointType;

        public SignPoint(Guid playerId, Point position, SignPointType pointType)
        {
            _position = position;
            _playerId = playerId;
            _pointType = pointType;            
        }

        public SignPoint(Guid playerId, int x, int y, SignPointType pointType) : this(playerId, new Point(x, y), pointType) { }


        public Point Position { get { return _position; } }

        public SignPointType PointType { get { return _pointType; } }

        public Guid PlayerId { get { return _playerId; } }


        public bool IsReadOnly => (_pointType == SignPointType.Sign || _pointType == SignPointType.MineUsed);

        //public bool IsEmpty => (PointType == SignPointType.Empty);


        public void ExplodeMine(Guid detonatedBy)
        {
            if (_pointType == SignPointType.MineNew)
            {
                _pointType = SignPointType.MineUsed;
                _playerId = detonatedBy;
            }
        }

        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
            {
                return false;
            }

            var c1 = (SignPoint)obj;

            return c1._position.Equals(this._position) && c1.PointType == this.PointType;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        /// <summary>
        /// Get relative direction between two points
        /// </summary>
        public static CombinationDirection operator ^(SignPoint c1, SignPoint c2)
        {
            return GetCombinationDirection(c1, c2);
        }

        public static bool operator ==(SignPoint c1, SignPoint c2)
        {
            return c1._playerId == c2._playerId 
                && c1._position == c2._position 
                && c1._pointType == c2._pointType;
        }

        public static bool operator !=(SignPoint c1, SignPoint c2)
        {
            return !(c1 == c2);
        }

        public static CombinationDirection GetCombinationDirection(SignPoint p1, SignPoint p2)
        {
            var point1 = p1.Position;
            var point2 = p2.Position;

            var dX = point2.X - point1.X;
            var dY = point2.Y - point1.Y;

            if (dX == 0 && dY == 0) { return CombinationDirection.SamePoint; }   //exclude current point

            if (Math.Abs(dX) <= 1 && Math.Abs(dY) <= 1) //only neghbour points                
            {
                if (dX == 0) { return CombinationDirection.Vertical; }
                if (dY == 0) { return CombinationDirection.Horizontal; }

                if (dX == dY) { return CombinationDirection.UpDownDiagonal; }
                if (dX == -dY) { return CombinationDirection.DownUpDiagonal; }
            }

            return CombinationDirection.Undefined;
        }
    }

    public enum SignPointType : byte
    {
        Empty = 0,
        Sign = 1,

        MineNew = 128,
        MineUsed = 129
    }
}


