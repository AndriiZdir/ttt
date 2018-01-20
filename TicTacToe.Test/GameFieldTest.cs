using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Drawing;
using TicTacToe.BL.Models;

namespace TicTacToe.Test
{
    [TestClass]
    public class GameFieldTest : GameField
    {
        [TestMethod]
        public void CheckGameFieldIdNotEmpty()
        {
            var gm = this;

            Assert.AreNotEqual(Guid.Empty, gm.GameId);
        }

        [TestMethod]
        public void CheckPointSignSet()
        {
            var gm = this;

            gm.SetPointSign(2, 2, DAL.Enums.GameRoomPlayerSign.Cross);

            var point = gm.GetSignPointByCoords(2, 2);

            Assert.IsFalse(point.IsEmpty);

            point = gm.GetSignPointByCoords(1, 2);

            Assert.IsTrue(point.IsEmpty);

            Assert.ThrowsException<ArgumentException>(() => { gm.SetPointSign(0, 0, DAL.Enums.GameRoomPlayerSign.Empty); });
        }

        [TestMethod]
        public void CheckBoundsIncrease()
        {
            var gm = this;

            gm.SetPointSign(0, 0, DAL.Enums.GameRoomPlayerSign.Cross);
            Assert.AreEqual(Rectangle.FromLTRB(0, 0, 0, 0), gm.Bounds);

            gm.SetPointSign(2, 9, DAL.Enums.GameRoomPlayerSign.Cross);
            Assert.AreEqual(Rectangle.FromLTRB(0, 0, 2, 9), gm.Bounds);

            gm.SetPointSign(-2, -2, DAL.Enums.GameRoomPlayerSign.Cross);
            Assert.AreEqual(Rectangle.FromLTRB(-2, -2, 2, 9), gm.Bounds);

            gm.SetPointSign(-10, -10, DAL.Enums.GameRoomPlayerSign.Cross);
            Assert.AreEqual(Rectangle.FromLTRB(-10, -10, 2, 9), gm.Bounds);

            Assert.ThrowsException<ArgumentOutOfRangeException>(() => { gm.SetPointSign(-21, -20, DAL.Enums.GameRoomPlayerSign.Poops); });
        }


        [TestMethod]
        public void CheckMaxBoundsViolation()
        {
            var gm = this;

            gm.SetPointSign(0, 0, DAL.Enums.GameRoomPlayerSign.Cross);
            Assert.AreEqual(Rectangle.FromLTRB(0, 0, 0, 0), gm.Bounds);

            gm.SetPointSign(2, 9, DAL.Enums.GameRoomPlayerSign.Cross);
            Assert.AreEqual(Rectangle.FromLTRB(0, 0, 2, 9), gm.Bounds);

            gm.SetPointSign(-2, -2, DAL.Enums.GameRoomPlayerSign.Cross);
            Assert.AreEqual(Rectangle.FromLTRB(-2, -2, 2, 9), gm.Bounds);

            Assert.ThrowsException<ArgumentOutOfRangeException>(() => { gm.SetPointSign(-999, 0, DAL.Enums.GameRoomPlayerSign.Poops); });
            Assert.ThrowsException<ArgumentOutOfRangeException>(() => { gm.SetPointSign(0, -999, DAL.Enums.GameRoomPlayerSign.Poops); });
            Assert.ThrowsException<ArgumentOutOfRangeException>(() => { gm.SetPointSign(999, 999, DAL.Enums.GameRoomPlayerSign.Poops); });
        }

        [TestMethod]
        public void TryToFillNotEmptyCell()
        {
            var gm = this;

            gm.SetPointSign(0, 0, DAL.Enums.GameRoomPlayerSign.Cross);
            Assert.AreEqual(Rectangle.FromLTRB(0, 0, 0, 0), gm.Bounds);

            gm.SetPointSign(2, 2, DAL.Enums.GameRoomPlayerSign.Zero);
            Assert.AreEqual(Rectangle.FromLTRB(0, 0, 2, 2), gm.Bounds);

            Assert.ThrowsException<Exception>(() => { gm.SetPointSign(2, 2, DAL.Enums.GameRoomPlayerSign.Zero); });
        }

        [TestMethod]
        public void TestGettingNeighbourPoints()
        {
            var gm = this;

            gm.SetPointSign(0, 0, DAL.Enums.GameRoomPlayerSign.Cross);
            gm.SetPointSign(1, 0, DAL.Enums.GameRoomPlayerSign.Zero);
            gm.SetPointSign(1, -1, DAL.Enums.GameRoomPlayerSign.Zero);
            gm.SetPointSign(0, 1, DAL.Enums.GameRoomPlayerSign.Cross);
            gm.SetPointSign(-1, 0, DAL.Enums.GameRoomPlayerSign.Cross);
            gm.SetPointSign(-1, 1, DAL.Enums.GameRoomPlayerSign.Zero);
            gm.SetPointSign(1, 1, DAL.Enums.GameRoomPlayerSign.Cross);
            

            gm.SetPointSign(0, 2, DAL.Enums.GameRoomPlayerSign.Cross);
            gm.SetPointSign(-2, 0, DAL.Enums.GameRoomPlayerSign.Cross);
            gm.SetPointSign(-1, 2, DAL.Enums.GameRoomPlayerSign.Zero);

            
            var testPoint = gm.GetSignPointByCoords(0, 0);

            int i = 0;

            foreach (var nPoint in gm.GetNeighbourPoints(testPoint, true))
            {
                Assert.AreEqual(testPoint.Sign, nPoint.Sign);

                var direction = Combination.GetCombinationDirection(nPoint.Position.X, nPoint.Position.Y, testPoint.Position.X, testPoint.Position.Y);

                Assert.AreNotEqual(direction, CombinationDirection.Undefined);

                i++;
            }

            Assert.AreEqual(3, i);

            i = 0;

            foreach (var nPoint in gm.GetNeighbourPoints(testPoint, false))
            {
                var direction = Combination.GetCombinationDirection(nPoint.Position.X, nPoint.Position.Y, testPoint.Position.X, testPoint.Position.Y);

                Assert.AreNotEqual(direction, CombinationDirection.Undefined);

                i++;
            }

            Assert.AreEqual(6, i);
        }
    }
}

