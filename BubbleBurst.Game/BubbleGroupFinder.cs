using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using BubbleBurst.Game.Extensions;

namespace BubbleBurst.Game
{
    public class BubbleGroupFinder
    {
        public BubbleGroupFinder(ImmutableBubbleBurstGrid grid, IEnumerable<BubbleGroup> parentsGroups)
        {
            _grid = grid;
            _stats = new Dictionary<Bubble, int>();
            //_parentGroups = parentsGroups ?? Enumerable.Empty<BubbleGroup>();
        }

        private readonly ImmutableBubbleBurstGrid _grid;
        private Dictionary<Bubble, int> _stats;
        private BubbleGroup _currentGroup;
        private IEnumerable<BubbleGroup> _parentGroups;

        public BubbleGroupFinderResult Find()
        {
            var groups = new List<BubbleGroup>();

            // If a group is still valid for this grid, we don't need to search for it again
            // Turns out this is a bad optimisation - needs profiling
            //groups.AddRange(_parentGroups.Where(x => x.IsValidFor(_grid)));

            for (int y = 0; y < _grid.Height; y++)
            {
                for (int x = 0; x < _grid.Width; x++)
                {
                    if(!_stats.ContainsKey(_grid[x,y])) _stats.Add(_grid[x,y], 1);
                    else _stats[_grid[x, y]]++;

                    if (!groups.Any(group => group.Locations.Contains(new Point(x, y))))
                    {
                        _currentGroup = new BubbleGroup();
                        FindBubbleGroup(x, y);
                        if (_currentGroup.Locations.Count > 1 && _currentGroup.Colour != Bubble.None)
                        {
                            groups.Add(_currentGroup);
                        }
                    }
                }
            }
            return new BubbleGroupFinderResult(groups, _stats);
        }

        private void FindBubbleGroup(int x, int y)
        {
            _currentGroup.Locations.Add(new Point(x, y));

            foreach (var match in FindNeighbours(x, y))
            {
                var current = new Point(match.X, match.Y);
                _currentGroup.Colour = _grid[x, y];
                if (_currentGroup.Locations.Contains(current)) continue;
                _currentGroup.Locations.Add(current);
                FindBubbleGroup(match.X, match.Y);
            }
        }

        private IEnumerable<Point> FindNeighbours(int x, int y)
        {
            var neighbourMatches = new List<Point>();

            var color = _grid[x, y];

            //neighbours - up, down, left, right
            //          (x,y+1)
            //(x-1,y)   (x,y)   (x+1,y)
            //          (x,y-1) 
            //if()
            var north = new Point(x, y + 1);
            var south = new Point(x, y - 1);
            var east = new Point(x - 1, y);
            var west = new Point(x + 1, y);

            //need to check if north/south/east/west are legal bubbles
            if (!_grid.IsLegal(north))
                north = Point.Empty;

            if (!_grid.IsLegal(south))
                south = Point.Empty;

            if (!_grid.IsLegal(east))
                east = Point.Empty;

            if (!_grid.IsLegal(west))
                west = Point.Empty;

            if (north != Point.Empty && _grid[north.X, north.Y] == color)
            {
                neighbourMatches.Add(north);
            }
            if (south != Point.Empty && _grid[south.X, south.Y] == color)
            {
                neighbourMatches.Add(south);
            }
            if (east != Point.Empty && _grid[east.X, east.Y] == color)
            {
                neighbourMatches.Add(east);
            }
            if (west != Point.Empty && _grid[west.X, west.Y] == color)
            {
                neighbourMatches.Add(west);
            }

            return neighbourMatches;
        }
    }
}