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

        public ImmutableBubbleBurstGrid(InternalGrid grid, IEnumerable<BubbleGroup> parentsFinder = null)
        {
            Grid = grid;
            Width = grid.Width();
            Height = grid.Height();
            _groupFinder = new BubbleGroupFinder(this, parentsFinder);

        }

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