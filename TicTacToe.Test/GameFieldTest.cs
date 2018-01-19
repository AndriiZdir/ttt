using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Drawing;
using TicTacToe.BL.Models;

namespace TicTacToe.Test
{
    [TestClass]
    public class GameFieldTest
    {
        [TestMethod]
        public void GameFieldId()
        {
            var gm = new GameField();

            Assert.AreNotEqual(Guid.Empty, gm.GameId);
        }

        [TestMethod]
        public void BoundsIncrease()
        {
            var gm = new GameField();

            gm.SetPointSign(0, 0, DAL.Enums.GameRoomPlayerSign.Cross);

            Assert.AreEqual(Rectangle.FromLTRB(0, 0, 0, 0), gm.Bounds);

            gm.SetPointSign(2, 2, DAL.Enums.GameRoomPlayerSign.Cross);

            Assert.AreEqual(Rectangle.FromLTRB(0, 0, 2, 2), gm.Bounds);

            gm.SetPointSign(-2, -2, DAL.Enums.GameRoomPlayerSign.Cross);

            Assert.AreEqual(Rectangle.FromLTRB(-2, -2, 2, 2), gm.Bounds);

            gm.SetPointSign(-20, -20, DAL.Enums.GameRoomPlayerSign.Cross);

            Assert.AreEqual(Rectangle.FromLTRB(-20, -20, 2, 2), gm.Bounds);            
        }

        [TestMethod]
        public void TryToFillNotEmptyCell()
        {
            var gm = new GameField();

            gm.SetPointSign(0, 0, DAL.Enums.GameRoomPlayerSign.Cross);

            Assert.AreEqual(Rectangle.FromLTRB(0, 0, 0, 0), gm.Bounds);

            gm.SetPointSign(2, 2, DAL.Enums.GameRoomPlayerSign.Zero);

            Assert.AreEqual(Rectangle.FromLTRB(0, 0, 2, 2), gm.Bounds);

            Assert.ThrowsException<Exception>(() => { gm.SetPointSign(2, 2, DAL.Enums.GameRoomPlayerSign.Zero); });
        }
    }
}
