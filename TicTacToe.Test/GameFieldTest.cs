using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Drawing;
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

            Assert.AreEqual(pnt1.PlayerId, pnt3.PlayerId);
            Assert.AreEqual(pnt2.PlayerId, pnt4.PlayerId);
            Assert.AreEqual(pnt1.PlayerId, pl2.Id);
            Assert.AreEqual(pnt2.PlayerId, pl1.Id);
            Assert.AreEqual(pnt5.PlayerId, pl1.Id);

        }

        //[TestMethod]
        //public void CheckBoundsIncrease()
        //{


        //    gm.SetPointSign(0, 0, DAL.Enums.GameRoomPlayerSign.Cross);
        //    Assert.AreEqual(Rectangle.FromLTRB(0, 0, 0, 0), gm.Bounds);

        //    gm.SetPointSign(2, 9, DAL.Enums.GameRoomPlayerSign.Cross);
        //    Assert.AreEqual(Rectangle.FromLTRB(0, 0, 2, 9), gm.Bounds);

        //    gm.SetPointSign(-2, -2, DAL.Enums.GameRoomPlayerSign.Cross);
        //    Assert.AreEqual(Rectangle.FromLTRB(-2, -2, 2, 9), gm.Bounds);

        //    gm.SetPointSign(-10, -10, DAL.Enums.GameRoomPlayerSign.Cross);
        //    Assert.AreEqual(Rectangle.FromLTRB(-10, -10, 2, 9), gm.Bounds);

        //    Assert.ThrowsException<ArgumentOutOfRangeException>(() => { gm.SetPointSign(-21, -20, DAL.Enums.GameRoomPlayerSign.Poops); });
        //}

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

        //[TestMethod]
        //public void TestCombinations()
        //{


        //    /*            
        //     *            
        //     *     XX       
        //     *    XX      
        //     *             
        //     *              
        //     */


        //    gm.SetPointSign(0, 0, DAL.Enums.GameRoomPlayerSign.Cross);
        //    gm.SetPointSign(1, 1, DAL.Enums.GameRoomPlayerSign.Cross);

        //    Assert.AreEqual(1, gm.Combinations.Count);

        //    gm.SetPointSign(0, 1, DAL.Enums.GameRoomPlayerSign.Cross);

        //    Assert.AreEqual(3, gm.Combinations.Count);

        //    gm.SetPointSign(-1, 0, DAL.Enums.GameRoomPlayerSign.Cross);

        //    Assert.AreEqual(5, gm.Combinations.Count);

        //    //gm.SetPointSign(1, 0, DAL.Enums.GameRoomPlayerSign.Cross);

        //    ////gm.SetPointSign(1, 0, DAL.Enums.GameRoomPlayerSign.Zero);
        //    ////gm.SetPointSign(1, -1, DAL.Enums.GameRoomPlayerSign.Zero);            
        //    ////gm.SetPointSign(-1, 1, DAL.Enums.GameRoomPlayerSign.Zero);

        //    //Assert.AreEqual(7, gm.Combinations.Count);
        //}
    }
}

