using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;

namespace TicTacToe.BL.Models
{
    public class Combination
    {
        public Combination()
        {
            Points = new List<SignPoint>();
            Bound = Rectangle.Empty;
            State = CombinationState.Open;
            Direction = CombinationDirection.Undefined;
        }

        public CombinationState State { get; set; }

        public CombinationDirection Direction { get; private set; }

        public Rectangle Bound { get; private set; }

        public List<SignPoint> Points { get; private set; }

        public bool IsSuitableCombinationFor(int x, int y)
        {
            if (State != CombinationState.Open && State != CombinationState.Half) { return false; }
            if (!Bound.Contains(x, y)) { return false; }

            return true;
        }

        public static CombinationDirection GetCombinationDirection(int x1, int y1, int x2, int y2)
        {
            var dX = x2 - x1;
            var dY = y2 - y1;

            if (Math.Abs(dX) <= 1 && Math.Abs(dY) <= 1 && !(dX == 0 && dY == 0))
            {
                if (dX == 0 && (dY == 1 || dY == -1)) { return CombinationDirection.Vertical; }
                if (dY == 0 && (dX == 1 || dX == -1)) { return CombinationDirection.Horizontal; }

                if (dX == dY) { return CombinationDirection.UpDownDiagonal; }
                if (dX == -dY) { return CombinationDirection.DownUpDiagonal; }
            }

            return CombinationDirection.Undefined;
        }
    }

    public enum CombinationState : byte
    {
        Closed,
        Half,
        Open,
        Completed
    }

    public enum CombinationDirection : byte
    {
        Undefined,
        Vertical,
        Horizontal,
        UpDownDiagonal,
        DownUpDiagonal        
    }
}
