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

        *    
        */

        public string CurrentTurnPlayerId { get; set; }

        public static GameCurrentStateModel FromGameField(GameField gameField, int skipPoints = 0, int skipCombinations = 0)
        {
            var model = new GameCurrentStateModel();



            return model;
        }

        public class GameState_Point
        {
            public int X { get; set; }
            public int Y { get; set; }
            public string PlayerId { get; set; }
            //public int Type { get; set; }
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
