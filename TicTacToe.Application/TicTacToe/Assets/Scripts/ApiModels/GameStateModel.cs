using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Scripts.ApiModels
{
    public class GameStateModel
    {
        public IEnumerable<GameState_Point> Points;
        public IEnumerable<GameState_Combination> Combinations;
        public IEnumerable<GameState_PlayerTable> Players;
        public GameState_GameBounds MoveBounds;
        public string CurrentTurnPlayerId;
        public GameFieldState GameState;        

        public class GameState_Point
        {
            public int X;
            public int Y;
            public string PlayerId;
            public byte Type { get; set; }
        }

        public class GameState_Combination
        {
            public int X1;
            public int Y1;
            public int X2;
            public int Y2;
            public string PlayerId;
        }

        public class GameState_PlayerTable
        {
            public string PlayerId;
            public int Points;
            public bool SkipsNextTurn;
            //public int MinesLeft;
        }

        public class GameState_GameBounds
        {
            public float Left;
            public float Top;
            public float Right;
            public float Bottom;
        }
    }

    public enum GameFieldState
    {
        New,
        Ready,
        Started,
        Completed
    }
}
