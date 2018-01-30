using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using TicTacToe.BL.Models;

namespace TicTacToe.Test
{
    //[TestClass]    
    public class PerformanceTest
    {
        const int normal_performance = 100;
        [TestMethod]
        public void CurrentTicks()
        {
            Stopwatch sw = new Stopwatch();
            sw.Start();
            for (int i = 0; i < 1_000_000; i++)
            {
                var ticks = DateTime.UtcNow.Ticks;
            }
            sw.Stop();
            
            Assert.IsTrue(sw.ElapsedMilliseconds < normal_performance, "HELLO");
        }
    }
}
