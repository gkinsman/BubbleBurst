using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BubbleBurst.Game;
using BubbleBurst.Game.Extensions;
using NUnit.Framework;

namespace BubbleBurst.Tests
{
    public class BubbleBurstGridTests
    {
        private Builder _grid;

        [TestFixtureSetUp]
        public void MyTestInitialize()
        {
            _grid = BubbleGridBuilder.Create(new[]
                                             {
                                                 new[] {Bubble.Blue, Bubble.Green, Bubble.Purple},
                                                 new[] {Bubble.Red, Bubble.Green, Bubble.Red},
                                                 new[] {Bubble.Red, Bubble.Yellow, Bubble.Yellow}
                                             }).ToBuilder();
        }

        [Test]
        public void BubbleGrid_ZeroIndexes_BottomRightReturned()
        {
            int x = 0;
            int y = 0;
            var actual = _grid[x, y];
            Assert.AreEqual(Bubble.Yellow, actual);
        }

        [Test]
        public void BubbleGrid_MidIndexes_MiddleReturned()
        {
            int x = 1;
            int y = 1;
            Bubble actual = _grid[x, y];
            Assert.AreEqual(Bubble.Green, actual);
        }

        [Test]
        public void BubbleGrid_TopIndexes_TopLeftReturned()
        {
            int x = 2;
            int y = 2;
            Bubble actual;
            actual = _grid[x, y];
            Assert.AreEqual(Bubble.Blue, actual);
        }

        [Test]
        public void BubbleGrid_CoalesceColumns_TwoAdjustments()
        {
            _grid.CoalesceColumns();

            _grid[1, 0] = Bubble.None;

            _grid.CoalesceColumns();
            var actual1 = _grid[1, 0];
            var actual2 = _grid[1, 1];
            var actual3 = _grid[1, 2];
            var expected1 = Bubble.Green;
            var expected2 = Bubble.Green;
            var expected3 = Bubble.None;

            Assert.AreEqual(expected1, actual1);
            Assert.AreEqual(expected2, actual2);
            Assert.AreEqual(expected3, actual3);
        }

        [Test]
        public void BubbleGrid_CoalesceColumns_NoAdjustments()
        {
            _grid[2, 2] = Bubble.None;

            _grid.CoalesceColumns();
            var actual1 = _grid[2, 2];
            var expected1 = Bubble.None;

            Assert.AreEqual(expected1, actual1);
        }

        [Test]
        public void BubbleGrid_CoalesceColumns_OneAdjustment()
        {
            _grid[0, 1] = Bubble.None;

            _grid.CoalesceColumns();
            var actual = _grid[0, 1];
            var expected = Bubble.Purple;

            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void RemoveEmptyColumn_Middle_LeftEmpty()
        {
            _grid[1, 1] = Bubble.None;
            _grid[1, 0] = Bubble.None;
            _grid[1, 2] = Bubble.None;

            _grid.RemoveEmptyColumns();

            var actual1 = _grid[1, 0];
            var actual2 = _grid[1, 1];
            var actual3 = _grid[1, 2];

            var expected1 = Bubble.Red;
            var expected2 = Bubble.Red;
            var expected3 = Bubble.Blue;

            Assert.AreEqual(expected1, actual1);
            Assert.AreEqual(expected2, actual2);
            Assert.AreEqual(expected3, actual3);
        }
    }
}