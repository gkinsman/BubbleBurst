using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using BubbleBurst.Game;
using Serilog;

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
            var root = new GameMove(_grid);

            GameMove topState = root;

            Func<GameMove, IEnumerable<GameMove>> topThreeSelectionStrategy =
                x => x.GridState.Groups
                    .OrderByDescending(y => y.Score).Take(3)
                    .Select(y => x.BurstBubble(y.Locations.First()));

            Func<GameMove, IEnumerable<GameMove>> allSelectionStrategy =
                x => x.GridState.Groups
                    .Select(y => x.BurstBubble(y.Locations.First()));

            var treeRoot = new LazyGeneratedTree<GameMove>(root, topThreeSelectionStrategy);

            var watch = new Stopwatch();
            watch.Start();

            long nodeCount = 0;
            foreach (var node in treeRoot.GetEnumerable(TreeTraversalType.BreadthFirst, TreeTraversalDirection.TopDown))
            {
                if (nodeCount%1000 == 0 && nodeCount > 0)
                    Log.Debug("{NodeCount} nodes processed in {TotalSeconds} seconds with max score of {TopScore}", 
                        nodeCount, watch.Elapsed.TotalSeconds, topState.Score);

                if (node.Value.Score > topState.Score)
                {
                    topState = node.Value;

                    Log.Information(
                        "Top score found: {TopScore} at depth {Depth} after {NodesProcessed} searched nodes in {TotalSeconds} seconds",
                        node.Value.Score,node.Value.Moves.Count, nodeCount, watch.Elapsed.TotalSeconds);
                }

                nodeCount++;
            }

            return topState;
        }
    }
}