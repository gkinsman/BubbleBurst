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
            var thisMoveCount = Moves.Count;
            var otherMoveCount = other.Moves.Count;

            // Easy cases
            if (thisMoveCount < otherMoveCount && Score > other.Score) return -1;
            if (thisMoveCount > otherMoveCount && Score < other.Score) return 1;
            if (thisMoveCount == otherMoveCount && Score == other.Score) return 0;

            /*
                Need to decide on the point value of a move. Essentially an average move score that 
                will let us evaluate the cost of a position. 
                A higher value means that deeper positions will require more points to beat a shallow one. 
                A lower value means that deeper positions with lower scores might get too much attention.
            */

            /*
                More Ideas:
                - Give weight to boards with fewer groups remaining to 'finish off' the game
                - Give weight to boards with groups of only two
            */

            //Lets start with 10 points.

            var thisScore = CalcPositionScore(thisMoveCount, Score);
            var otherScore = CalcPositionScore(otherMoveCount, other.Score);

            if (thisScore == otherScore) return 0;
            return thisScore > otherScore ? -1 : 1;
        }

        private static int CalcPositionScore(int moveCount, int score)
        {
            return Math.Abs(score - (moveCount *10));
        }
    }
}