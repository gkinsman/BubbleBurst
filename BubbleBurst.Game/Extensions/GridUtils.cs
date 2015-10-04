using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using static System.String;
using InternalGrid =
    System.Collections.Immutable.ImmutableList<System.Collections.Immutable.ImmutableList<BubbleBurst.Game.Bubble>>;
using InternalGridBuilder =
    System.Collections.Immutable.ImmutableList
        <System.Collections.Immutable.ImmutableList<BubbleBurst.Game.Bubble>.Builder>;

namespace BubbleBurst.Game.Extensions
{
    public static class BubbleGridExtensions
    {
        /// <summary>
        /// Transposes the given point such that the origin (0,0) is
        /// in the bottom right corner.
        /// </summary>
        /// <param name="point"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <returns></returns>
        public static Point Transpose(this Point point, int width, int height)
        {
            var newCol = (width - 1) - point.X;
            var newRow = (height - 1) - point.Y;
            Debug.Assert(newCol < width && newCol >= 0);
            Debug.Assert(newRow < height && newRow >= 0);
            return new Point(newCol, newRow);
        }

        public static InternalGrid ToImmutableGrid(this InternalGridBuilder.Builder builder)
        {
            var innerImmutables = builder.Select(x => x.ToImmutable()).ToArray();
            return ImmutableList.Create(innerImmutables);
        }

        public static InternalGridBuilder.Builder ToGridBuilder(this InternalGrid grid)
        {
            var builder = ImmutableList.CreateBuilder<ImmutableList<Bubble>.Builder>();
            builder.AddRange(grid.Select(x => x.ToBuilder()));

            return builder;
        }

        public static int Height(this InternalGrid grid)
        {
            return grid.Count;
        }

        public static int Width(this InternalGrid grid)
        {
            return grid[0].Count;
        }

        public static int Height(this InternalGridBuilder.Builder grid)
        {
            return grid.Count;
        }

        public static int Width(this InternalGridBuilder.Builder grid)
        {
            return grid[0].Count;
        }

        public static void Display(this ImmutableBubbleBurstGrid grid)
        {
            for (var row = 0; row < grid.Height; row++)
            {
                for (var col = 0; col < grid.Width; col++)
                {
                    var current = grid[col, row].ToString()[0];
                    if (current != 'N')
                    {
                        switch (current)
                        {
                            case 'B':
                                Console.BackgroundColor = ConsoleColor.Blue;
                                break;
                            case 'Y':
                                Console.BackgroundColor = ConsoleColor.Yellow;
                                break;
                            case 'G':
                                Console.BackgroundColor = ConsoleColor.Green;
                                break;
                            case 'C':
                                Console.BackgroundColor = ConsoleColor.Cyan;
                                break;
                            case 'R':
                                Console.BackgroundColor = ConsoleColor.Red;
                                break;
                        }

                        Console.Write(current);
                        Console.BackgroundColor = ConsoleColor.Black;
                        Console.Write(" ");
                    }
                    else
                    {
                        Console.Write("  ");
                    }
                }
                Console.Write(row + " ");
                Console.Write(Environment.NewLine);
            }
            for (var a = 0; a < grid.Width; a++)
            {
                Console.Write(a + " ");
            }
            Console.WriteLine();
        }

        public static string DisplayPoints(this List<Point> points)
        {
            return Join(",", points.Select(x => $"({x.X},{x.Y})"));
        }

        public static void PushColumnsRight(this Builder grid)
        {
            var adjustment = 0;

            for (var col = grid.Width -1; col >= 0; col--)
            {
                //check if every element in the column is none
                var allNone = true;
                for (var a = 0; a < grid.Height; a++)
                {
                    if (grid[col, a] != Bubble.None)
                    {
                        allNone = false;
                    }
                }

                if (!allNone) continue;

                //empty column
                for (var i = col; i >= 0; i--)
                {
                    var allNone2 = true;
                    for (var row = grid.Height -1; row >= 0; row--)
                    {
                        if (grid[i, row] == Bubble.None) continue;

                        allNone2 = false;
                        var colour = grid[i, row];
                        grid[i, row] = Bubble.None;
                        grid[i + adjustment, row] = colour;
                    }
                    if (allNone2)
                        adjustment++;
                }
            }
        }

        public static Tuple<ImmutableBubbleBurstGrid, int> RemoveGroup(this ImmutableBubbleBurstGrid grid, Point point)
        {
            if (!grid.IsLegal(point))
                throw new ArgumentException($"Invalid move - Point ({point.X},{point.Y}) is invalid in this grid");

            var pointsGroup = grid.Groups.FirstOrDefault(x => x.Locations.Contains(point));
            if (pointsGroup == null)
                throw new ArgumentException(
                    $"Invalid move - Point ({point.X},{point.Y}) does not belong to a valid group");

            var gridBuilder = grid.ToBuilder();
            foreach (var bubble in pointsGroup.Locations)
            {
                gridBuilder[bubble.X, bubble.Y] = Bubble.None;
            }

            gridBuilder.JumpTillTheresNoGaps();
            gridBuilder.PushColumnsRight();

            return Tuple.Create(gridBuilder.ToImmutable(), pointsGroup.Score);
        }

        public static void JumpTillTheresNoGaps(this Builder grid)
        {
            for (var col = 0; col < grid.Width; col++)
            {
                var adjustment = 0;
                for (var row = grid.Height -1; row >= 0 ; row--)
                {
                    var adjusted = false;
                    if (grid[col, row] == Bubble.None)
                    {
                        adjustment++;
                        adjusted = true;
                    }
                    if (adjustment == 0) continue;

                    if (grid[col, row] == Bubble.None)
                    {
                        if (!adjusted)
                        {
                            adjustment++;
                        }
                        continue;
                    }
                    var colour = grid[col, row];
                    grid[col, row] = Bubble.None;
                    grid[col, row + adjustment] = colour;
                }
            }
        }


        public static bool IsLegal(this ImmutableBubbleBurstGrid grid, Point point)
        {
            var returnBool = true;

            if (point.X < 0 || point.X >= grid.Width)
            {
                returnBool = false;
            }
            else if (point.Y < 0 || point.Y >= grid.Height)
            {
                returnBool = false;
            }

            return returnBool;
        }

        public static string ToTestString(this ImmutableList<ImmutableList<Bubble>> grid)
        {
            var builder = new StringBuilder();
            foreach (var row in grid)
            {
                foreach (var col in row)
                {
                    builder.Append(col + " ");
                }
                builder.Append("-");
            }
            return builder.ToString();
        }
    }
}