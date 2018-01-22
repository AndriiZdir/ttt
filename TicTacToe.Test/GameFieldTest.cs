using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Drawing;
using System.Linq;
using TicTacToe.BL.Models;

namespace TicTacToe.Test
{
    [TestClass]
    public class GameFieldTest : GameField
    {
        GameFieldTest gm;

        public GameFieldTest() : base(200)
        {
            gm = this;
        }

        [TestMethod]
        public void CheckGameFieldIdNotEmpty()
        {
            Assert.AreNotEqual(Guid.Empty, gm.GameId);
        }

        [TestMethod]
        public void TestPlayerAdding()
        {
            Assert.AreEqual(GameFieldState.New, gm.State);

            gm.AddPlayerToField(Guid.NewGuid());

            Assert.AreEqual(GameFieldState.New, gm.State);

            gm.AddPlayerToField(Guid.NewGuid());

            Assert.AreEqual(GameFieldState.Ready, gm.State);

            gm.AddPlayerToField(Guid.NewGuid());

            Assert.AreEqual(3, gm.Players.Count);
        }

        [TestMethod]
        public void TestPlayerChanging()
        {
            Assert.AreEqual(GameFieldState.New, gm.State);

            var p1 = gm.AddPlayerToField(Guid.NewGuid());

            Assert.AreEqual(GameFieldState.New, gm.State);

            var p2 = gm.AddPlayerToField(Guid.NewGuid());

            Assert.AreEqual(GameFieldState.Ready, gm.State);

            var p3 = gm.AddPlayerToField(Guid.NewGuid());

            Assert.AreEqual(3, gm.Players.Count);

            Assert.AreEqual(p1, gm.CurrentTurnPlayer);
            UpdateNextTurnPlayer();
            Assert.AreEqual(p2, gm.CurrentTurnPlayer);
            UpdateNextTurnPlayer();
            Assert.AreEqual(p3, gm.CurrentTurnPlayer);
            p3.SkipNextTurn = true;
            UpdateNextTurnPlayer();
            Assert.AreEqual(p1, gm.CurrentTurnPlayer);
            UpdateNextTurnPlayer();
            Assert.AreEqual(p2, gm.CurrentTurnPlayer);
            UpdateNextTurnPlayer();
            Assert.AreEqual(p1, gm.CurrentTurnPlayer);
            p1.SkipNextTurn = true;
            UpdateNextTurnPlayer();
            Assert.AreEqual(p2, gm.CurrentTurnPlayer);
            UpdateNextTurnPlayer();
            Assert.AreEqual(p3, gm.CurrentTurnPlayer);
            UpdateNextTurnPlayer();
            Assert.AreEqual(p2, gm.CurrentTurnPlayer);
        }

        [TestMethod]
        public void CheckPointSignSet()
        {
            Assert.ThrowsException<InvalidOperationException>(() => { gm.SetPointSign(3, 4); });

            var pl1 = gm.AddPlayerToField(Guid.NewGuid());

            Assert.ThrowsException<InvalidOperationException>(() => { gm.SetPointSign(3, 4); });

            var pl2 = gm.AddPlayerToField(Guid.NewGuid());

            var pnt0 = gm.SetPointSign(3, 4);

            Assert.ThrowsException<Exception>(() => { gm.SetPointSign(3, 4); });

            var pnt1 = gm.SetPointSign(0, 0);

            var pnt2 = gm.SetPointSign(1, 0);

            var pnt3 = gm.SetPointSign(1, -1);
            pl2.SkipNextTurn = true;
            Assert.IsTrue(pl2.SkipNextTurn);

            var pnt4 = gm.SetPointSign(0, 1);     
            
            var pnt5 = gm.SetPointSign(3, 1);
            Assert.IsFalse(pl2.SkipNextTurn);

            Assert.AreEqual(pnt1.Player, pnt3.Player);
            Assert.AreEqual(pnt2.Player, pnt4.Player);
            Assert.AreEqual(pnt1.Player, pl2);
            Assert.AreEqual(pnt2.Player, pl1);
            Assert.AreEqual(pnt5.Player, pl1);

        }

        [TestMethod]
        public void CheckBoundsIncrease()
        {
            gm.AddPlayerToField(Guid.NewGuid());
            gm.AddPlayerToField(Guid.NewGuid());

            gm.SetPointSign(0, 0);
            Assert.AreEqual(Rectangle.FromLTRB(0, 0, 0, 0), gm.Bounds);

            gm.SetPointSign(2, 9);
            Assert.AreEqual(Rectangle.FromLTRB(0, 0, 2, 9), gm.Bounds);

            gm.SetPointSign(-2, -2);
            Assert.AreEqual(Rectangle.FromLTRB(-2, -2, 2, 9), gm.Bounds);

            gm.SetPointSign(-10, -10);
            Assert.AreEqual(Rectangle.FromLTRB(-10, -10, 2, 9), gm.Bounds);

            Assert.ThrowsException<ArgumentOutOfRangeException>(() => { gm.SetPointSign(-21, -20); });
        }

        //[TestMethod]
        //public void CheckMaxBoundsViolation()
        //{


        //    gm.SetPointSign(0, 0, DAL.Enums.GameRoomPlayerSign.Cross);
        //    Assert.AreEqual(Rectangle.FromLTRB(0, 0, 0, 0), gm.Bounds);

        //    gm.SetPointSign(2, 9, DAL.Enums.GameRoomPlayerSign.Cross);
        //    Assert.AreEqual(Rectangle.FromLTRB(0, 0, 2, 9), gm.Bounds);

        //    gm.SetPointSign(-2, -2, DAL.Enums.GameRoomPlayerSign.Cross);
        //    Assert.AreEqual(Rectangle.FromLTRB(-2, -2, 2, 9), gm.Bounds);

        //    Assert.ThrowsException<ArgumentOutOfRangeException>(() => { gm.SetPointSign(-999, 0, DAL.Enums.GameRoomPlayerSign.Poops); });
        //    Assert.ThrowsException<ArgumentOutOfRangeException>(() => { gm.SetPointSign(0, -999, DAL.Enums.GameRoomPlayerSign.Poops); });
        //    Assert.ThrowsException<ArgumentOutOfRangeException>(() => { gm.SetPointSign(999, 999, DAL.Enums.GameRoomPlayerSign.Poops); });
        //}

        //[TestMethod]
        //public void TryToFillNotEmptyCell()
        //{


        //    gm.SetPointSign(0, 0, DAL.Enums.GameRoomPlayerSign.Cross);
        //    Assert.AreEqual(Rectangle.FromLTRB(0, 0, 0, 0), gm.Bounds);

        //    gm.SetPointSign(2, 2, DAL.Enums.GameRoomPlayerSign.Zero);
        //    Assert.AreEqual(Rectangle.FromLTRB(0, 0, 2, 2), gm.Bounds);

        //    Assert.ThrowsException<Exception>(() => { gm.SetPointSign(2, 2, DAL.Enums.GameRoomPlayerSign.Zero); });
        //}

        [TestMethod]
        public void TestGettingNeighbourPoints()
        {            
            var pl1 = gm.AddPlayerToField(Guid.NewGuid());
            var pl2 = gm.AddPlayerToField(Guid.NewGuid());

            Assert.AreEqual(GameFieldState.Ready, gm.State);
            
            gm.SetPointSign(0, 0);            
            gm.SetPointSign(1, 0);            
            gm.SetPointSign(1, -1);
            gm.SetPointSign(0, 1);
            gm.SetPointSign(-1, 0);
            gm.SetPointSign(-1, 1);
            gm.SetPointSign(1, 1);  
            

            var testPoint = gm.GetPointByCoords(0, 0);

            int i = 0;

            foreach (var nPoint in gm.GetNeighbourPoints(testPoint, true))
            {
                Assert.AreEqual(testPoint.PointType, nPoint.PointType);

                var direction = testPoint ^ nPoint;
                var reverseDirection = nPoint ^ testPoint;

                Assert.AreEqual(direction, reverseDirection);
                Assert.AreNotEqual(direction, CombinationDirection.Undefined);

                i++;
            }

            Assert.AreEqual(3, i);

            i = 0;

            foreach (var nPoint in gm.GetNeighbourPoints(testPoint, false))
            {
                var direction = testPoint ^ nPoint;

                Assert.AreNotEqual(direction, CombinationDirection.Undefined);

                i++;
            }

            Assert.AreEqual(6, i);
        }

        [TestMethod]
        public void TestCombinations1()
        {
            
            var pl1 = gm.AddPlayerToField(Guid.NewGuid());
            _currentTurnPlayer = pl1;
            _gameState = GameFieldState.Ready;
            
            Assert.AreEqual(GameFieldState.Ready, gm.State);

            /*            
             *            
             *     XX       
             *    XXXXX     
             *             
             *              
             */


            var point = gm.SetPointSign(0, 0);
            point = gm.SetPointSign(1, 1);

            Assert.AreEqual(1, gm.Combinations.Count);

            point = gm.SetPointSign(0, 1);

            Assert.AreEqual(3, gm.Combinations.Count);

            point = gm.SetPointSign(-1, 0);

            Assert.AreEqual(5, gm.Combinations.Count);

            point = gm.SetPointSign(3, 0);

            Assert.AreEqual(5, gm.Combinations.Count);

            point = gm.SetPointSign(2, 0);

            Assert.AreEqual(7, gm.Combinations.Count);

            point = gm.SetPointSign(1, 0);

            Assert.AreEqual(8, gm.Combinations.Count);


            //Combination merging test

            point = gm.SetPointSign(-1, 10);
            point = gm.SetPointSign(0, 10);

            Assert.AreEqual(9, gm.Combinations.Count);

            point = gm.SetPointSign(3, 10);

            Assert.AreEqual(9, gm.Combinations.Count);

            point = gm.SetPointSign(2, 10);

            Assert.AreEqual(10, gm.Combinations.Count);

            point = gm.SetPointSign(1, 10);

            Assert.AreEqual(9, gm.Combinations.Count);


            //9 point combination

            point = gm.SetPointSign(-1, 15);
            point = gm.SetPointSign(0, 15);
            point = gm.SetPointSign(1, 15);
            point = gm.SetPointSign(2, 15);

            Assert.AreEqual(10, gm.Combinations.Count);

            var ninePointCombination = point.GetPointCombinationForDirection(CombinationDirection.Horizontal);
            Assert.AreEqual(4, ninePointCombination.Points.Count);
            Assert.AreEqual(CombinationState.Open, ninePointCombination.State);

            point = gm.SetPointSign(4, 15);
            point = gm.SetPointSign(5, 15);
            point = gm.SetPointSign(6, 15);
            point = gm.SetPointSign(7, 15);

            Assert.AreEqual(11, gm.Combinations.Count);

            ninePointCombination = point.GetPointCombinationForDirection(CombinationDirection.Horizontal);
            Assert.AreEqual(4, ninePointCombination.Points.Count);
            Assert.AreEqual(CombinationState.Open, ninePointCombination.State);

            point = gm.SetPointSign(3, 15);

            Assert.AreEqual(10, gm.Combinations.Count);

            ninePointCombination = point.GetPointCombinationForDirection(CombinationDirection.Horizontal);

            Assert.AreEqual(9, ninePointCombination.Points.Count);
            Assert.AreEqual(CombinationState.Completed, ninePointCombination.State);

            point = gm.SetPointSign(9, 15);
            point = gm.SetPointSign(10, 15);

            Assert.AreEqual(11, gm.Combinations.Count);

            point = gm.SetPointSign(8, 15);

            Assert.AreEqual(11, gm.Combinations.Count);
        }

        [TestMethod]
        public void TestPlayerCombinationsAndGameCompleting()
        {

            var pl1 = gm.AddPlayerToField(Guid.NewGuid());
            var pl2 = gm.AddPlayerToField(Guid.NewGuid());

            Assert.AreEqual(PlayerState.InGame, pl1.State);
            Assert.AreEqual(PlayerState.InGame, pl2.State);

            Assert.AreEqual(GameFieldState.Ready, gm.State);

            /*            
             *      X      
             *      XOOO     
             *      X O  
             *      XO       
             *      X        
             */

            var p1point = gm.SetPointSign(0, 0);
            var p2point = gm.SetPointSign(1, 1);

            Assert.AreEqual(0, gm.Combinations.Count);

            p1point = gm.SetPointSign(0, 2);
            p2point = gm.SetPointSign(2, 1);

            Assert.AreEqual(1, gm.Combinations.Count);

            p1point = gm.SetPointSign(0, 1);
            p2point = gm.SetPointSign(3, 1);

            Assert.AreEqual(2, gm.Combinations.Count);

            p1point = gm.SetPointSign(0, -1);
            p2point = gm.SetPointSign(2, 0);

            Assert.AreEqual(5, gm.Combinations.Count);

            p1point = gm.SetPointSign(0, -2);
            p2point = gm.SetPointSign(1, -1);

            Assert.AreEqual(5, gm.Combinations.Count);
            Assert.AreEqual(1, gm.Combinations.Count(x => x.State == CombinationState.Completed));
            Assert.AreEqual(1, pl1.Points);

            gm.CompleteTheGame();

            Assert.AreEqual(PlayerState.Winner, pl1.State);
            Assert.AreEqual(PlayerState.Loser, pl2.State);
        }
    }
}

