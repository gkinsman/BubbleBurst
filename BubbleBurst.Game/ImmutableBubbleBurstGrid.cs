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

        //private readonly Dictionary<Bubble, int> _bubbleCounts;

        public ImmutableBubbleBurstGrid(InternalGrid grid)
        {
            Grid = grid;
            Width = grid.Width();
            Height = grid.Height();
            _groupFinder = new BubbleGroupFinder(this);

            //_bubbleCounts = GetStats(grid);
        }

/*        private static Dictionary<Bubble, int> GetStats(InternalGrid grid)
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
        }*/

        /// <summary>
        /// Returns the element where [0,0] is at the bottom right
        /// </summary>
        /// <param name="col">represents the horizontal index</param>
        /// <param name="row">represents the vertical index</param>
        /// <returns></returns>
        public Bubble this[int col, int row] => Grid[row][col];

        public override string ToString()
        {
            return GetHashCode() + ": " + Grid.ToTestString();
        }

        public override int GetHashCode()
        {
            return Grid.GetHashCode();
        }

        private InternalGrid Grid { get; }

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
}