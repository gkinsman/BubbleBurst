using System;
using System.IO;
using System.Threading;
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

            Console.ReadLine();

            var tokenSource = new CancellationTokenSource(TimeSpan.FromMinutes(20));
            var cancellationToken = tokenSource.Token;

            var solutionFileName = "Solution.json";

            File.Delete(solutionFileName);
            using (var fileStream = File.OpenWrite(solutionFileName))
            using(var writer = new StreamWriter(fileStream))
            {

                var solver = new GridSolver(grid, writer, cancellationToken);
                var solution = solver.Solve();

                solution.GridState.Display();
            }


            Console.ReadLine();
        }
    }
}