using System.Drawing;
using BubbleBurst.Game.Extensions;

namespace BubbleBurst.Game
{
    public static class GameMoveExtensions
    {
        public static GameMove BurstBubble(this GameMove lastMove, Point point)
        {
            var currentGrid = lastMove.GridState;
            var result = currentGrid.RemoveGroup(point);

            return new GameMove(result.Item1, point, result.Item2);
        }
    }

    public class GameMove
    {
        internal GameMove(ImmutableBubbleBurstGrid gridState, Point point, int score)
        {
            Point = point;
            Score = score;
            GridState = gridState;
        }

        public GameMove(ImmutableBubbleBurstGrid grid)
        {
            Score = 0;
            GridState = grid;
        }

        public Point? Point { get; }
        public int Score { get; }

        public ImmutableBubbleBurstGrid GridState { get; }
    }
}