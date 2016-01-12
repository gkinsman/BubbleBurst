using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Threading;
using BubbleBurst.Bot;
using BubbleBurst.Game;
using BubbleBurst.Game.Extensions;
using CLAP;
using CLAP.Validation;
using Newtonsoft.Json;
using Serilog;

namespace BubbleBurst.Runner
{
    public class BubbleBurstApp
    {
        [Verb(IsDefault = true, Description = "Solve for the given grid and depth penalty")]
        public static void Run([DefaultValue("Game1.txt")] string gameFile, [DefaultValue("18")] int depthPenalty,
            [DefaultValue("50000")] int maxMoves)
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

                writer.Write(JsonConvert.SerializeObject(solution.Moves));

                solution.GridState.Display();
            }
        }

        [Verb]
        public static void Play([Required] string gameFile, [Required, FileExists] string movesFile)
        {
            Log.Logger = new LoggerConfiguration()
                .WriteTo.LiterateConsole()
                .MinimumLevel.Debug()
                .CreateLogger();

            ImmutableBubbleBurstGrid grid;
            using (var stream = File.OpenRead($"Games//{gameFile}"))
            {
                grid = BubbleGridBuilder.Create(new StreamReader(stream));
            }

            var state = new GameMove(grid);

            var movesJson = File.ReadAllText(movesFile);

            var moves = JsonConvert.DeserializeObject<List<Point>>(movesJson);


            state.GridState.Display();

            var lastScore = 0;
            foreach (var move in moves)
            {
                Console.WriteLine();
                Console.WriteLine($"({move.X}, {move.Y})");
                state = state.BurstBubble(move);
                Console.WriteLine($"{state.Score - lastScore} points scored this time ({state.Score} total)");
                state.GridState.Display();
                Thread.Sleep(1000);

                lastScore = state.Score;
            }
        }

        [Error]
        public void Error(ExceptionContext e)
        {
            Log.Error(e.Exception, "An error occurred");
        }
    }
}