using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;

namespace TicTacToe.BL.Models
{
    public class Combination
    {
        private int _id;

        public Combination(int id, CombinationDirection direction)
        {
            _id = id;
            Points = new List<SignPoint>();
            Bound = Rectangle.Empty;
            State = CombinationState.Open;
            Direction = direction;            
        }

        public int Id { get { return _id; } }

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
        public bool IsSuitableFor(SignPoint point)
        {
            if (State != CombinationState.Open && State != CombinationState.Half) { return false; }

            foreach(var p in Points)
            {
                var direction = point.GetDirectionWith(p);

                if (Direction == direction) { return true; }
            }            

            return false;
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
