using System;
using System.Drawing;
using System.Reflection;
using BubbleBurst.Game;
using BubbleBurst.Game.Extensions;
using NUnit.Framework;

namespace BubbleBurst.Tests
{
    public class GameMoveTests
    {
        private ImmutableBubbleBurstGrid _grid;

        [SetUp]
        public void Setup()
        {
            _grid = BubbleGridBuilder.Create(new[]
                                             {
                                                 new[] {Bubble.Blue, Bubble.Green, Bubble.Cyan, Bubble.Blue},
                                                 new[] {Bubble.Red, Bubble.Green, Bubble.Red, Bubble.Green},
                                                 new[] {Bubble.Red, Bubble.Yellow, Bubble.Yellow, Bubble.Cyan, },
                                                 new[] {Bubble.Red, Bubble.Cyan, Bubble.Cyan, Bubble.Cyan, }
                                             });
        }

        [Test]
        public void CreateFromGrid_TakeMove_ShouldPopCorrectly()
        {

            var expected = BubbleGridBuilder.Create(new[]
                                             {
                                                 new[] {Bubble.Blue,Bubble.None , Bubble.None, Bubble.Blue},
                                                 new[] {Bubble.Red, Bubble.Green, Bubble.Cyan, Bubble.Green},
                                                 new[] {Bubble.Red, Bubble.Green, Bubble.Red, Bubble.Cyan},
                                                 new[] {Bubble.Red, Bubble.Cyan, Bubble.Cyan, Bubble.Cyan, }
                                             });

            var move = new GameMove(_grid);

            _grid.Display();

            Console.WriteLine("Taking move (2,2)..");
            var next = move.BurstBubble(new Point(2, 2));

            next.GridState.Display();

            Assert.IsTrue(expected.Equals(next.GridState));
        }

        [Test]
        public void CreateFromGrid_TakeTwoMoves_ShouldInheritState()
        {

            var expected = BubbleGridBuilder.Create(new[]
                                             {
                                                 new[] {Bubble.Blue, Bubble.None, Bubble.None, Bubble.None, },
                                                 new[] {Bubble.Red, Bubble.Green, Bubble.Cyan,Bubble.None },
                                                 new[] {Bubble.Red, Bubble.Green, Bubble.Red, Bubble.Blue },
                                                 new[] {Bubble.Red, Bubble.Yellow, Bubble.Yellow, Bubble.Green }
                                             });

            var secondExpected = BubbleGridBuilder.Create(new[]
                                             {
                                                 new[] {Bubble.Blue, Bubble.None, Bubble.None, Bubble.None,},
                                                 new[] {Bubble.Red, Bubble.None, Bubble.None, Bubble.None},
                                                 new[] {Bubble.Red, Bubble.Green, Bubble.Cyan, Bubble.Blue},
                                                 new[] {Bubble.Red, Bubble.Green, Bubble.Red, Bubble.Green}
                                             });

            var move = new GameMove(_grid);

            _grid.Display();

            Console.WriteLine("Taking move (3,3)..");
            var threeThree = move.BurstBubble(new Point(3,3));

            threeThree.GridState.Display();

            Assert.IsTrue(expected.Equals(threeThree.GridState));

            Console.WriteLine("Taking move (2,3)");
            var twoThree = threeThree.BurstBubble(new Point(2, 3));

            twoThree.GridState.Display();

            Assert.IsTrue(secondExpected.Equals(twoThree.GridState));
        }
    }
}