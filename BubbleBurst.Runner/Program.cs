using System;
using System.IO;
using BubbleBurst.Bot;
using BubbleBurst.Game;
using BubbleBurst.Game.Extensions;

namespace BubbleBurst.Runner
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            ImmutableBubbleBurstGrid grid;
            using (var stream = File.OpenRead("Games//Game1.txt"))
            {
                grid = BubbleGridBuilder.Create(new StreamReader(stream));
            }

            var solver = new GridSolver(grid);

            var solution = solver.Solve();

            solution.GridState.Display();

            Console.ReadLine();
        }
    }
}