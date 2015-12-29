using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using BubbleBurst.Game.Extensions;

namespace BubbleBurst.Game  
{
    public static class GameMoveExtensions
    {
        public static GameMove BurstBubble(this GameMove lastMove, Point point)
        {
            var currentGrid = lastMove.GridState;
            var result = currentGrid.RemoveGroup(point);
            var moves = lastMove.Moves.Concat(new[] {new GameMove.PointAndColour(point, result.Item3), }).ToList();

            return new GameMove(result.Item1, moves, result.Item2 + lastMove.Score);
        }
    }

    public class GameMove : IComparable<GameMove>
    {
        public struct PointAndColour
        {
            public PointAndColour(Point point, Bubble colour)
            {
                Point = point;
                Colour = colour;
            }

            public Point Point { get; }
            public Bubble Colour { get; }
        }

        internal GameMove(ImmutableBubbleBurstGrid gridState, IList<PointAndColour> moves, int score)
        {
            Score = score;
            Moves = moves;
            GridState = gridState;
        }

        public GameMove(ImmutableBubbleBurstGrid grid)
        {
            Score = 0;
            GridState = grid;
            Moves = new List<PointAndColour>();
        }

        public IList<PointAndColour> Moves { get; private set; }

        public int Score { get; }

        public ImmutableBubbleBurstGrid GridState { get; }

        public int CompareTo(GameMove other)
        {
            throw new InvalidOperationException("Must use custom comparer");
        }
    }
}