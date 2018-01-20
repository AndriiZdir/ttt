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

        public void AddPoint(SignPoint point)
        {
            if (Points.Any(x => x == point)) { throw new ArgumentException("This point is already in this combination", nameof(point)); }

            Points.Add(point);
        }

        /// <summary>
        /// Check if point is suitable for adding to this combination.
        /// </summary>
        /// <param name="x">x coord</param>
        /// <param name="y">y coord</param>
        /// <returns>Is suitable</returns>
        public bool IsSuitableFor(int x, int y)
        {
            if (State != CombinationState.Open && State != CombinationState.Half) { return false; }
            

            return true;
        }

        public static CombinationDirection GetCombinationDirection(int x1, int y1, int x2, int y2)
        {
            var dX = x2 - x1;
            var dY = y2 - y1;

            if(dX == 0 && dY == 0) { return CombinationDirection.SamePoint; }   //exclude current point

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
        DownUpDiagonal,
        
        SamePoint = 0b11111111
    }
}
