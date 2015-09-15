using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.Remoting.Channels;
using BubbleBurst.Game.Extensions;

namespace BubbleBurst.Game
{
    public static class BubbleGridBuilder
    {
        public static ImmutableBubbleBurstGrid Create(string filename)
        {
            using (var reader = new StreamReader(new FileStream(filename, FileMode.Open)))
            {
                return Create(reader);
            }
        }

        public static ImmutableBubbleBurstGrid Create(StreamReader reader)
        {
            var lines = new List<string>();

            string line;
            while ((line = reader.ReadLine()) != null)
            {
                lines.Add(line);
            }

            // Ensure all rows are the same size
            var width = lines[0].Length;
            Debug.Assert(lines.All(x => x.Length == width));

            var height = lines.Count;

            var grid = ImmutableList.CreateBuilder<ImmutableList<Bubble>.Builder>();

            for (var i = 0; i < height; i++)
            {
                grid[i] = ImmutableList.CreateBuilder<Bubble>();
            }

            lines = lines.Select(x => x.ToUpper()).ToList();

            //create grid from list of string values
            for (var i = 0; i < height; i++)
            {
                for (var j = 0; j < width; j++)
                {
                    grid[i][j] = lines[i][j].ToBubble();
                }
            }

            return new ImmutableBubbleBurstGrid(grid.ToImmutableGrid());
        }

        public static ImmutableBubbleBurstGrid Create(Bubble[][] grid)
        {
            var result = ImmutableList.CreateBuilder<ImmutableList<Bubble>.Builder>();

            var width = grid.Length;
            var height = grid[0].Length;

            for (var i = 0; i < height; i++)
            {
                result.Add(ImmutableList.CreateBuilder<Bubble>());
            }

            //create grid from list of string values
            for (var i = 0; i < height; i++)
            {
                result[i] = ImmutableList.CreateBuilder<Bubble>();
                result[i].AddRange(grid[i]);
            }

            return new ImmutableBubbleBurstGrid(result.ToImmutableGrid());
        }
    }
}