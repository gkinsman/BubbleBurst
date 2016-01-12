using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using BubbleBurst.Game;
using BubbleBurst.Game.Extensions;
using Newtonsoft.Json;
using Serilog;
using Serilog.Formatting.Json;

namespace BubbleBurst.Bot
{
    public class GridSolver
    {
        private readonly ImmutableBubbleBurstGrid _grid;
        private readonly StreamWriter _writer;
        private readonly int _depthPenalty;

        public static long MoveCount { get; set; }

        public GridSolver(ImmutableBubbleBurstGrid grid, StreamWriter writer, int depthPenalty)
        {
            _grid = grid;
            _writer = writer;
            _depthPenalty = depthPenalty;
        }

        public GameMove Solve(int maxMoves)
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

            Func<GameMove, IEnumerable<GameMove>> selectLeastCommonBubblesFirst =
                move =>
                {
                    var chosen =
                        move.GridState.Statistics.OrderBy(x => x.Value)
                            .Where(x => move.GridState.Groups.Any(group => group.Colour == x.Key))
                            .Select(x => x.Key).Take(2);

                    var taken = move.GridState.Groups.Where(x => chosen.Contains(x.Colour)).Take(3);
                    return taken.Select(x => move.BurstBubble(x.Locations.First()));
                };
                
            var treeRoot = new LazyGeneratedTree<GameMove>(root, selectLeastCommonBubblesFirst);

            var comparer = new DepthPenaliserComparer(_depthPenalty);

            var scores = new List<TopScore>();

            var watch = new Stopwatch();
            watch.Start();

            int nodeCount = 0;
            var moves = treeRoot.GetPriorityFirstEnumerable(comparer);
            foreach (var node in moves)
            {
                if (nodeCount > maxMoves)
                {
                    WriteResults(scores);

                    return topState;
                }

                if (nodeCount%1000 == 0 && nodeCount > 0)
                    Log.Debug("{NodeCount} nodes processed in {TotalSeconds} seconds with max score of {TopScore}", 
                        nodeCount, watch.Elapsed.TotalSeconds, topState.Score);

                if (node.Value.Score > topState.Score)
                {
                    topState = node.Value;

                    Log.Information(
                        "Top score found: {TopScore} at depth {Depth} after {NodesProcessed} searched nodes in {TotalSeconds} seconds",
                        node.Value.Score,node.Value.Moves.Count, nodeCount, watch.Elapsed.TotalSeconds);

                    node.Value.GridState.Display();

                    scores.Add(new TopScore(node.Value.Score, node.Value.Moves.Count, watch.Elapsed, nodeCount));
                }

                nodeCount++;
            }

            return topState;
        }

        private void WriteResults(List<TopScore> scores)
        {
            _writer.Write(JsonConvert.SerializeObject(scores));
        }

        class TopScore
        {
            public TopScore(int score, int depth, TimeSpan elapsed, int totalMoveCount)
            {
                Score = score;
                Depth = depth;
                Elapsed = elapsed;
                TotalMoveCount = totalMoveCount;
            }

            public int Score { get; set; }
            public int Depth { get; set; }
            public TimeSpan Elapsed { get; set; }
            public int TotalMoveCount { get; set; }

        }
    }
}