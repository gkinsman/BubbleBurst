using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using BubbleBurst.Game.Extensions;

namespace BubbleBurst.Game
{
    public class Builder
    {
        private readonly ImmutableList<ImmutableList<Bubble>>.Builder _gridBuilder;

        internal Builder(ImmutableList<ImmutableList<Bubble>>.Builder grid)
        {
            this._gridBuilder = grid;
        }

        public ImmutableBubbleBurstGrid ToImmutable(IEnumerable<BubbleGroup> parentGroups = null)
        {
            return new ImmutableBubbleBurstGrid(_gridBuilder.ToImmutable(), parentGroups);
        }

        public Bubble this[int col, int row]
        {
            get
            {
                return _gridBuilder[row][col];
            }
            set
            {
                var list = _gridBuilder[row].ToBuilder();
                list[col] = value;
                _gridBuilder[row] = list.ToImmutable();
            }
        }

        public int Width => _gridBuilder.Width();
        public int Height => _gridBuilder.Height();
    }
}