using System;
using System.Collections.Generic;
using System.Linq;
using BubbleBurst.Game;

namespace BubbleBurst.Bot
{
    public class GridSolver
    {
        private readonly ImmutableBubbleBurstGrid _grid;

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

            var treeRoot = new LazyGeneratedTree<GameMove>(first, topThreeSelectionStrategy);

            foreach (var item in treeRoot.GetEnumerable(TreeTraversalType.BreadthFirst, TreeTraversalDirection.TopDown))
            {
                if (item.Value.Score > topState.Score)
                {
                    topState = item.Value;
                    Console.WriteLine($"Top score found: {item.Value.Score}");
                }
            }

            return topState;
        }
    }
}