using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TicTacToe.BL.Models;

namespace TicTacToe.API.Areas.Models
{
    public class GameCurrentStateModel
    {

        /*
         * 
        
        points list
            -coords
            -player id
            -point type (except new mines)

        complete combinations list
            -player id
            -start-end point
            
        turn player
            -player id

        players frags
            -player id
            -count

        allowed game bounds
            -left top right bottom

        game state

        *    
        */

        public IEnumerable<GameState_Point> Points { get; set; }
        public IEnumerable<GameState_Combination> Combinations { get; set; }
        public IEnumerable<GameState_PlayerTable> Players { get; set; }        
        public GameState_GameBounds MoveBounds { get; set; }
        public string CurrentTurnPlayerId { get; set; }
        public GameFieldState GameState { get; set; }

        public static GameCurrentStateModel FromGameField(GameField gameField, int skipPoints = 0, int skipCombinations = 0)
        {
            var model = new GameCurrentStateModel();

            model.Points = gameField.Points
                //.Where(x=> x.PointType == SignPointType.Sign || x.PointType == SignPointType.MineUsed)
                .Select(x => new GameState_Point { X = x.Position.X, Y = x.Position.Y, PlayerId = x.Player.Id, Type = (byte)x.PointType })
                .ToList();

            model.Combinations = gameField.Combinations
                .Where(x => x.State == CombinationState.Completed)
                .Select(x => new GameState_Combination { PlayerId = x.Points[0].Player.Id })
                .ToList();

            model.Players = gameField.Players
                .Select(x => new GameState_PlayerTable { PlayerId = x.Id, Points = x.Points, SkipsNextTurn = x.SkipNextTurn })
                .ToList();

            model.CurrentTurnPlayerId = gameField.CurrentTurnPlayer.Id;

            var moveBounds = gameField.MoveBounds;
            model.MoveBounds = new GameState_GameBounds()
            {
                Left = moveBounds.Left,
                Top = moveBounds.Top,
                Right = moveBounds.Right,
                Bottom = moveBounds.Bottom
            };

            model.GameState = gameField.State;

            return model;
        }

        public class GameState_Point
        {
            public int X { get; set; }
            public int Y { get; set; }
            public string PlayerId { get; set; }
            public byte Type { get; set; }
        }

        public class GameState_Combination
        {
            public int X1 { get; set; }
            public int Y1 { get; set; }
            public int X2 { get; set; }
            public int Y2 { get; set; }
            public string PlayerId { get; set; }
        }

        public class GameState_PlayerTable
        {
            public string PlayerId { get; set; }
            public int Points { get; set; }
            public bool SkipsNextTurn { get; set; }
            //public int MinesLeft { get; set; }
        }

        public class GameState_GameBounds
        {
            public int Left { get; set; }
            public int Top { get; set; }
            public int Right { get; set; }
            public int Bottom { get; set; }
        }
    }
}
