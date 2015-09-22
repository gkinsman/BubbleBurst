using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Tracing;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
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

        [SetUp]
        public void Setup()
        {
            _grid = BubbleGridBuilder.Create(new[]
                                             {
                                                 new[] {Bubble.Blue, Bubble.Green, Bubble.Purple, Bubble.Blue},
                                                 new[] {Bubble.Red, Bubble.Green, Bubble.Red, Bubble.Green},
                                                 new[] {Bubble.Red, Bubble.Yellow, Bubble.Yellow, Bubble.Blue, }
                                             }).ToBuilder();

        }

        [Test]
        public void BubbleGrid_Builder_ShouldBuildCorrectly()
        {
            _grid.ToImmutable().Display();

            Assert.AreEqual(Bubble.Blue, _grid[3, 2]);
            Assert.AreEqual(Bubble.Green, _grid[3, 1]);
            Assert.AreEqual(Bubble.Blue, _grid[3, 0]);

            Assert.AreEqual(Bubble.Yellow, _grid[2,2]);
            Assert.AreEqual(Bubble.Red, _grid[2,1]);
            Assert.AreEqual(Bubble.Purple, _grid[2,0]);

            Assert.AreEqual(Bubble.Yellow, _grid[1,2]);
            Assert.AreEqual(Bubble.Green, _grid[1,1]);
            Assert.AreEqual(Bubble.Green, _grid[1,0]);

            Assert.AreEqual(Bubble.Red, _grid[0,2]);
            Assert.AreEqual(Bubble.Red, _grid[0,1]);
            Assert.AreEqual(Bubble.Blue, _grid[0,0]);
        }

        [Test]
        public void BubbleGrid_ZeroIndexes_BottomRightReturned()
        {
            int x = 0;
            int y = 0;
            var builderActual = _grid[x, y];
            var immutableActual = _grid.ToImmutable()[x, y];

            _grid.ToImmutable().Display();

            Assert.AreEqual(Bubble.Blue, builderActual);
            Assert.AreEqual(Bubble.Blue, immutableActual);
        }

        [Test]
        public void BubbleGrid_MidIndexes_MiddleReturned()
        {
            int x = 1;
            int y = 1;
            Bubble builderActual = _grid[x, y];
            var immutableActual = _grid.ToImmutable()[x, y];

            _grid.ToImmutable().Display();

            Assert.AreEqual(Bubble.Green, builderActual);
            Assert.AreEqual(Bubble.Green, immutableActual);
        }

        [Test]
        public void BubbleGrid_TopIndexes_TopRightReturned()
        {
            int x = 3;
            int y = 2;
            var builderActual = _grid[x, y];
            var immutableActual = _grid.ToImmutable()[x, y];

            _grid.ToImmutable().Display();

            Assert.AreEqual(Bubble.Blue, builderActual);
            Assert.AreEqual(Bubble.Blue, immutableActual);
        }

        [Test]
        public void BubbleGrid_JumpOnTheGaps_TwoIndependantAdjustments()
        {
            _grid[0, 2] = Bubble.None;
            _grid[0, 1] = Bubble.None;

            _grid.ToImmutable().Display();

            Console.WriteLine("Jumping on the gaps...");

            _grid.JumpTillTheresNoGaps();

            var actual1 = _grid[0, 0];
            var actual2 = _grid[0, 1];
            var actual3 = _grid[0, 2];
            var expected1 = Bubble.None;
            var expected2 = Bubble.None;
            var expected3 = Bubble.Blue;

            _grid.ToImmutable().Display();

            Assert.AreEqual(expected1, actual1);
            Assert.AreEqual(expected2, actual2);
            Assert.AreEqual(expected3, actual3);
        }

        [Test]
        public void BubbleGrid_JumpTillTheresNoGaps_TwoAdjustments()
        {
            _grid[0, 2] = Bubble.None;

            _grid.ToImmutable().Display();

            Console.WriteLine("Jumping on the gaps...");

            _grid.JumpTillTheresNoGaps();

            var actual1 = _grid[0, 0];
            var actual2 = _grid[0, 1];
            var actual3 = _grid[0, 2];
            var expected1 = Bubble.None;
            var expected2 = Bubble.Blue;
            var expected3 = Bubble.Red;

            _grid.ToImmutable().Display();

            Assert.AreEqual(expected1, actual1);
            Assert.AreEqual(expected2, actual2);
            Assert.AreEqual(expected3, actual3);
        }

        [Test]
        public void BubbleGrid_JumpTillTheresNoGaps_NoAdjustments()
        {
            _grid[2, 0] = Bubble.None;

            _grid.JumpTillTheresNoGaps();
            var actual1 = _grid[2, 0];
            var expected1 = Bubble.None;

            _grid.ToImmutable().Display();

            Assert.AreEqual(expected1, actual1);
        }

        [Test]
        public void BubbleGrid_JumpTillTheresNoGaps_OneAdjustment()
        {
            _grid[0, 1] = Bubble.None;

            _grid.ToImmutable().Display();

            Console.WriteLine("Jumping on the gaps...");

            _grid.JumpTillTheresNoGaps();
            var actual = _grid[0, 1];
            var expected = Bubble.Blue;

            _grid.ToImmutable().Display();

            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void RemoveEmptyColumn_Middle_LeftEmpty()
        {
            _grid[1, 1] = Bubble.None;
            _grid[1, 0] = Bubble.None;
            _grid[1, 2] = Bubble.None;

            _grid.ToImmutable().Display();

            Console.WriteLine("Removing empty columns...");

            _grid.PushColumnsRight();

            _grid.ToImmutable().Display();

            Assert.AreEqual(Bubble.Blue, _grid[1, 0]);
            Assert.AreEqual(Bubble.Red, _grid[1, 1]);
            Assert.AreEqual(Bubble.Red, _grid[1, 2]);
        }

        [Test]
        public void RemoveEmptyColumn_MiddleRight_ShouldShiftOtherRowsTwo()
        {
            _grid[2, 1] = Bubble.None;
            _grid[2, 0] = Bubble.None;
            _grid[2, 2] = Bubble.None;

            _grid.ToImmutable().Display();

            Console.WriteLine("Removing empty columns...");

            _grid.PushColumnsRight();

            _grid.ToImmutable().Display();

            Assert.AreEqual(Bubble.Blue, _grid[1, 0]);
            Assert.AreEqual(Bubble.Red, _grid[1, 1]);
            Assert.AreEqual(Bubble.Red, _grid[1, 2]);

            Assert.AreEqual(Bubble.Green, _grid[2, 0]);
            Assert.AreEqual(Bubble.Green, _grid[2, 1]);
            Assert.AreEqual(Bubble.Yellow, _grid[2, 2]);
        }

        [Test]
        public void RemoveAndJump_RemoveBottomRightTriangle_ShouldShiftDownAndRight()
        {
            _grid[3,2] = Bubble.None;
            _grid[2,2] = Bubble.None;
            _grid[1,2] = Bubble.None;
            _grid[3,1] = Bubble.None;
            _grid[2,1] = Bubble.None;
            _grid[3,0] = Bubble.None;

            var expected = BubbleGridBuilder.Create(new[]
                                             {
                                                 new[] {Bubble.None, Bubble.Blue, Bubble.None, Bubble.None},
                                                 new[] {Bubble.None, Bubble.Red, Bubble.Green, Bubble.None},
                                                 new[] {Bubble.None, Bubble.Red, Bubble.Green, Bubble.Purple, }
                                             });

            _grid.ToImmutable().Display();

            _grid.JumpTillTheresNoGaps();
            _grid.PushColumnsRight();

            
            Console.WriteLine("Actual:");
            _grid.ToImmutable().Display();

            Console.WriteLine("Expected:");
            expected.Display();

            Assert.IsTrue(_grid.ToImmutable().Equals(expected));
        } 
    }
}