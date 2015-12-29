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
        public static void Run([DefaultValue("Game1.txt")] string gameFile, [DefaultValue("11")] int depthPenalty)
        {
            ImmutableBubbleBurstGrid grid;
            using (var stream = File.OpenRead($"Games//{gameFile}"))
            {
                grid = BubbleGridBuilder.Create(new StreamReader(stream));
            }

            var tokenSource = new CancellationTokenSource(TimeSpan.FromMinutes(5));
            var cancellationToken = tokenSource.Token;

            var solutionFileName = $"Solution_{gameFile.Replace(".txt","")}_Depth-{depthPenalty}.json";

            File.Delete(solutionFileName);
            using (var fileStream = File.OpenWrite(solutionFileName))
            using (var writer = new StreamWriter(fileStream))
            {
                var solver = new GridSolver(grid, writer, cancellationToken, depthPenalty);
                var solution = solver.Solve();

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