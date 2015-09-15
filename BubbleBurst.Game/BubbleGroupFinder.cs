using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using BubbleBurst.Game.Extensions;

namespace BubbleBurst.Game
{
    public class BubbleGroupFinder
    {
        public BubbleGroupFinder(ImmutableBubbleBurstGrid grid)
        {
            _grid = grid;
        }

        private readonly ImmutableBubbleBurstGrid _grid;
        private BubbleGroup _currentGroup;

        public IEnumerable<BubbleGroup> Find()
        {
            var groups = new List<BubbleGroup>();

            //iterate over grid
            for (int i = 0; i < _grid.Height; i++)
            {
                for (int j = 0; j < _grid.Width; j++)
                {
                    //if the current point isnt already part of a bubble group
                    if (!groups.Any(x => x.Points.Contains(new Point(j, i))))
                    {
                        //reset current group
                        _currentGroup = new BubbleGroup();
                        FindBubbleGroup(j, i);
                        if (_currentGroup.Points.Count > 1 && _currentGroup.Colour != Bubble.None)
                        {
                            //new group!
                            groups.Add(_currentGroup);
                            yield return _currentGroup;
                        }
                    }
                }
            }
        }

        private void FindBubbleGroup(int x, int y)
        {
            _currentGroup.Points.Add(new Point(x, y));

            foreach (var match in FindNeighbours(x, y))
            {
                var current = new Point(match.X, match.Y);
                _currentGroup.Colour = _grid[x, y];
                if (_currentGroup.Points.Contains(current)) continue;
                _currentGroup.Points.Add(current);
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