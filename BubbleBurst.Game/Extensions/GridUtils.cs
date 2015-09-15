using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
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
            return grid[0].Count;
        }

        public static int Width(this InternalGrid grid)
        {
            return grid.Count;
        }

        public static int Height(this InternalGridBuilder.Builder grid)
        {
            return grid[0].Count;
        }

        public static int Width(this InternalGridBuilder.Builder grid)
        {
            return grid.Count;
        }

        public static void Display(this ImmutableBubbleBurstGrid grid)
        {
            for (int i = grid.Height - 1; i >= 0; i--)
            {
                for (int j = grid.Width - 1; j >= 0; j--)
                {
                    var current = grid[j, i].ToString()[0];
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
                            case 'P':
                                Console.BackgroundColor = ConsoleColor.DarkMagenta;
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
                Console.Write((grid.Height - 1) - i + " ");
                Console.Write(Environment.NewLine);
            }
            for (int a = 0; a < grid.Width; a++)
            {
                Console.Write((grid.Width - 1) - a + " ");
            }
            Console.WriteLine();
        }

        public static string DisplayPoints(this List<Point> points)
        {
            var retVal = "";

            foreach (var item in points)
            {
                retVal = String.Concat(retVal, String.Format("({0},{1}), ", item.X, item.Y));
            }
            retVal = retVal.Trim(',');

            return retVal;
        }

        /// <summary>
        /// All mutator methods accept a BubbleGrid and return the same. Grids are immutable
        /// </summary>
        /// <param name="grid"></param>
        /// <returns></returns>
        public static void RemoveEmptyColumns(this Builder grid)
        {
            int adjustment = 0;

            for (int k = 0; k < grid.Width; k++)
            {
                //check if every element in the column is none
                bool allNone = true;
                for (int a = 0; a < grid.Height; a++)
                {
                    if (grid[k, a] != Bubble.None)
                    {
                        allNone = false;
                    }
                }

                if (!allNone) continue;
                adjustment++;

                //empty column
                for (int i = k + 1; i < grid.Width; i++)
                {
                    bool allNone2 = true;
                    for (int row = 0; row < grid.Height; row++)
                    {
                        if (grid[i, row] != Bubble.None)
                        {
                            allNone2 = false;
                            var colour = grid[i, row];
                            grid[i, row] = Bubble.None;
                            grid[i - adjustment, row] = colour;
                        }
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

            var pointsGroup = grid.Groups.FirstOrDefault(x => x.Points.Contains(point));
            if (pointsGroup == null)
                throw new ArgumentException(
                    $"Invalid move - Point ({point.X},{point.Y}) does not belong to a valid group");


            var gridBuilder = grid.ToBuilder();
            foreach (var bubble in pointsGroup.Points)
            {
                gridBuilder[bubble.X, bubble.Y] = Bubble.None;
            }

            gridBuilder.CoalesceColumns();
            gridBuilder.RemoveEmptyColumns();

            return Tuple.Create(gridBuilder.ToImmutable(), pointsGroup.Score);
        }

        public static void CoalesceColumns(this Builder grid)
        {
            for (int col = 0; col < grid.Width; col++)
            {
                int adjustment = 0;
                for (int row = 0; row < grid.Height; row++)
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
                    grid[col, row - adjustment] = colour;
                }
            }
        }


        public static bool IsLegal(this ImmutableBubbleBurstGrid grid, Point point)
        {
            bool returnBool = true;

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