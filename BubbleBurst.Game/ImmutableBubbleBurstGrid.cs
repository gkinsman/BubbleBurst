using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Configuration;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using BubbleBurst.Game.Extensions;
using InternalGrid =
    System.Collections.Immutable.ImmutableList<System.Collections.Immutable.ImmutableList<BubbleBurst.Game.Bubble>>;
using InternalGridBuilder =
    System.Collections.Immutable.ImmutableList
        <System.Collections.Immutable.ImmutableList<BubbleBurst.Game.Bubble>.Builder>;

namespace BubbleBurst.Game
{
    public class ImmutableBubbleBurstGrid : IEquatable<ImmutableBubbleBurstGrid>
    {
        public Builder ToBuilder()
        {
            return new Builder(Grid.ToGridBuilder());
        }

        public int Width { get; }
        public int Height { get; }

        private IEnumerable<BubbleGroup> _groups;
        public IEnumerable<BubbleGroup> Groups => _groups ?? (_groups = _groupFinder.Find());

        public bool GameEnded => !Groups.Any();

        private readonly BubbleGroupFinder _groupFinder;

        private readonly Dictionary<Bubble, int> _bubbleCounts;

        public ImmutableBubbleBurstGrid(InternalGrid grid)
        {
            Grid = grid;
            Width = grid.Width();
            Height = grid.Height();
            _groupFinder = new BubbleGroupFinder(this);

            _bubbleCounts = GetCounts(grid);
        }

        private static Dictionary<Bubble, int> GetCounts(InternalGrid grid)
        {
            var dict = new Dictionary<Bubble, int>();

            for (int i = 0; i < grid.Height(); i++)
            {
                for (int j = 0; j < grid.Width(); j++)
                {
                    var key = grid[i][j];
                    if (dict.ContainsKey(key)) dict[key] += 1;
                    else dict.Add(key, 1);
                }
            }

            return dict;
        }

        /// <summary>
        /// Returns the element where [0,0] is at the bottom right
        /// </summary>
        /// <param name="col">represents the horizontal index</param>
        /// <param name="row">represents the vertical index</param>
        /// <returns></returns>
        public Bubble this[int col, int row]
        {
            get
            {
                var transposedPoint = new Point(col, row).Transpose(Width, Height);
                return Grid[transposedPoint.X][transposedPoint.Y];
            }
        }

        public override string ToString()
        {
            return GetHashCode() + ": " + Grid.ToTestString();
        }

        public override int GetHashCode()
        {
            return Grid.GetHashCode();
        }

        private InternalGrid Grid { get; set; }

        public bool Equals(ImmutableBubbleBurstGrid other)
        {
            if (Grid.Count != other.Grid.Count)
                return false;

            for (int i = 0; i < Height; i++)
            {
                for (int j = 0; j < Width; j++)
                {
                    if (Grid[i][j] != other.Grid[i][j])
                        return false;
                }
            }

            return true;
        }
    }

    public class Builder
    {
        private readonly InternalGridBuilder.Builder _gridBuilder;

        internal Builder(InternalGridBuilder.Builder grid)
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
                var point = new Point(col, row).Transpose(_gridBuilder.Count, _gridBuilder[0].Count);
                return _gridBuilder[point.X][point.Y];
            }
            set
            {
                var point = new Point(col, row).Transpose(_gridBuilder.Count, _gridBuilder[0].Count);
                this._gridBuilder[point.X][point.Y] = value;
            }
        }

        public int Width => _gridBuilder.Width();
        public int Height => _gridBuilder.Height();
    }
}