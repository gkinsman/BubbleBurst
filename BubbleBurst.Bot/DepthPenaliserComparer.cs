using System;
using System.Collections.Generic;
using BubbleBurst.Game;

namespace BubbleBurst.Bot
{
    public class DepthPenaliserComparer : IComparer<SimpleTreeNode<GameMove>>
    {
        private readonly int _depthPenalty;

        public DepthPenaliserComparer(int depthPenalty)
        {
            _depthPenalty = depthPenalty;
        }

        public int Compare(SimpleTreeNode<GameMove> x, SimpleTreeNode<GameMove> y)
        {
            var first = x.Value;
            var second = y.Value;

            var thisMoveCount = first.Moves.Count;
            var otherMoveCount = second.Moves.Count;

            // Easy cases
            if (thisMoveCount < otherMoveCount && first.Score > second.Score) return -1;
            if (thisMoveCount > otherMoveCount && first.Score < second.Score) return 1;
            if (thisMoveCount == otherMoveCount && first.Score == second.Score) return 0;

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

            var thisScore = CalcPositionScore(thisMoveCount, first.Score);
            var otherScore = CalcPositionScore(otherMoveCount, second.Score);

            if (thisScore == otherScore) return 0;
            return thisScore > otherScore ? -1 : 1;
        }

        private int CalcPositionScore(int moveCount, int score)
        {
            return Math.Abs(score - (moveCount*_depthPenalty));
        }
    }
}