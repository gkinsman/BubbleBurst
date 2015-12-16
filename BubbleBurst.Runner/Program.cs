using System;
using System.IO;
using BubbleBurst.Bot;
using BubbleBurst.Game;
using BubbleBurst.Game.Extensions;
using Serilog;

namespace BubbleBurst.Runner
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            Log.Logger = new LoggerConfiguration()
                .WriteTo.LiterateConsole()
                .MinimumLevel.Debug()
                .CreateLogger();

            ImmutableBubbleBurstGrid grid;
            using (var stream = File.OpenRead("Games//Game3.txt"))
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