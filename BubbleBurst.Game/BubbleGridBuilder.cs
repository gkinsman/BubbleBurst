using System;
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

            var width = lines[0].Length;
            Debug.Assert(lines.All(x => x.Length == width));

            var array = lines.Select(x => x.Select(c => c.ToBubble()).ToArray()).ToArray();

            return Create(array);
        }

        public static ImmutableBubbleBurstGrid Create(Bubble[][] grid)
        {
            var result = ImmutableList.CreateBuilder<ImmutableList<Bubble>.Builder>();

            var height = grid.Length;

            for (var i = 0; i < height; i++)
            {
                result.Add(ImmutableList.CreateBuilder<Bubble>());
            }

            for (var i = 0; i < height; i++)
            {
                Debug.WriteLine($"Adding {string.Join(",", grid[i])} to position ({i})");

                result[i].AddRange(grid[i]);
            }

            return new ImmutableBubbleBurstGrid(result.ToImmutableGrid());
        }
    }
}