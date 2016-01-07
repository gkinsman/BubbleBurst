using System;
using System.IO;
using System.Threading;
using BubbleBurst.Bot;
using BubbleBurst.Game;
using BubbleBurst.Game.Extensions;
using CLAP;
using CLAP.Validation;
using Serilog;

namespace BubbleBurst.Runner
{
    public class BubbleBurstApp
    {
        [Verb(IsDefault = true, Description = "Solve for the given grid and depth penalty")]
        public static void Run([DefaultValue("Game3.txt")] string gameFile, [DefaultValue("18")] int depthPenalty, [DefaultValue("20000")] int maxMoves)
        {
            string gameId = gameFile.Replace(".txt", "");

            Log.Logger = new LoggerConfiguration()
                .WriteTo.LiterateConsole()
                .WriteTo.File($"Log-{gameId}_Depth-{depthPenalty}")
                .MinimumLevel.Debug()
                .CreateLogger();

            ImmutableBubbleBurstGrid grid;
            using (var stream = File.OpenRead($"Games//{gameFile}"))
            {
                grid = BubbleGridBuilder.Create(new StreamReader(stream));
            }

            var solutionFileName = $"Solution_{gameId}_Depth-{depthPenalty}.json";
            
            File.Delete(solutionFileName);
            using (var fileStream = File.OpenWrite(solutionFileName))
            using (var writer = new StreamWriter(fileStream))
            {
                var solver = new GridSolver(grid, writer, depthPenalty);
                var solution = solver.Solve(maxMoves);

                solution.GridState.Display();
            }
        }

        [Error]
        public void Error(ExceptionContext e)
        {
            Log.Error(e.Exception, "An error occurred");
        }

    }
}