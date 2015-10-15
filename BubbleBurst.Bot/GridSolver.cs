using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using BubbleBurst.Game;

namespace BubbleBurst.Bot
{
    public class GridSolver
    {
        private readonly ImmutableBubbleBurstGrid _grid;

        public static long MoveCount { get; set; }

        public GridSolver(ImmutableBubbleBurstGrid grid)
        {
            _grid = grid;
        }

        public GameMove Solve()
        {
            var first = new GameMove(_grid);

            GameMove topState = first;

            Func<GameMove, IEnumerable<GameMove>> topThreeSelectionStrategy =
                x => x.GridState.Groups
                      .OrderByDescending(y => y.Score).Take(3)
                      .Select(y => x.BurstBubble(y.Locations.First()));

            Func<GameMove, IEnumerable<GameMove>> allSelectionStrategy =
                x => x.GridState.Groups
                      .Select(y => x.BurstBubble(y.Locations.First()));

            var treeRoot = new LazyGeneratedTree<GameMove>(first, topThreeSelectionStrategy);

            var watch = new Stopwatch();
            watch.Start();

            var increment = 10;
            Parallel.ForEach(treeRoot.GetEnumerable(TreeTraversalType.BreadthFirst, TreeTraversalDirection.TopDown),
                (node, state) =>
                {
                    var totalSeconds = Math.Floor(watch.Elapsed.TotalSeconds);
                    if ((int)totalSeconds == increment)
                    {
                        Console.WriteLine($"{MoveCount} moves in {totalSeconds}");
                        increment += 10;
                    }

                    if (node.Value.Score > topState.Score)
                    {
                        topState = node.Value;
                        Console.WriteLine($"Top score found: {node.Value.Score}");
                        Console.WriteLine(string.Join(", ",
                            topState.Moves.Select(x => $"({x.Point.X},{x.Point.Y})({x.Colour})")));
                        Console.WriteLine($"Move count: {MoveCount}");
                        Console.WriteLine($"Elapsed: {watch.Elapsed.TotalSeconds}");
                    }
                });

            return topState;
        }
    }
}