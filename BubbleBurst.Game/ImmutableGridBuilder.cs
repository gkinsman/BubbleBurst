using System.Collections.Immutable;
using BubbleBurst.Game.Extensions;

namespace BubbleBurst.Game
{
    public class Builder
    {
        private readonly ImmutableList<ImmutableList<Bubble>.Builder>.Builder _gridBuilder;

        internal Builder(ImmutableList<ImmutableList<Bubble>.Builder>.Builder grid)
        {
            this._gridBuilder = grid;
        }

        public ImmutableBubbleBurstGrid ToImmutable()
        {
            return new ImmutableBubbleBurstGrid(_gridBuilder.ToImmutableGrid());
        }

        public Bubble this[int col, int row]
        {
            get
            {
                return _gridBuilder[row][col];
            }
            set
            {
                this._gridBuilder[row][col] = value;
            }
        }

        public int Width => _gridBuilder.Width();
        public int Height => _gridBuilder.Height();
    }
}