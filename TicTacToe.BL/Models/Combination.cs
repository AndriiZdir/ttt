using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;

namespace TicTacToe.BL.Models
{
    public class Combination
    {
        private readonly int _competedRowSize;

        public Combination(CombinationDirection direction, int competedRowSize = 5)
        {
            Points = new List<SignPoint>();
            State = CombinationState.Open;
            Direction = direction;
            _competedRowSize = competedRowSize;
        }

        public CombinationState State { get; set; }

        public CombinationDirection Direction { get; private set; }        

        public List<SignPoint> Points { get; private set; }

        public void AddPoint(SignPoint point)
        {
            if (Points.Any(x => x == point)) { throw new ArgumentException("This point is already in this combination", nameof(point)); }

            Points.Add(point);

            if (Points.Count >= _competedRowSize)
            {
                State = CombinationState.Completed;
                point.Player.AddPoints(1);
            }
        }

        public bool IsReadOnly => (State == CombinationState.Completed || State == CombinationState.Closed);

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
