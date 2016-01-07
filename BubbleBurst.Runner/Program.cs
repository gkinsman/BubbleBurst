using System;
using System.IO;
using System.Threading;
using BubbleBurst.Bot;
using BubbleBurst.Game;
using BubbleBurst.Game.Extensions;
using CLAP;
using Serilog;

namespace BubbleBurst.Runner
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            Parser.RunConsole<BubbleBurstApp>(args);

            Console.ReadLine();
        }
    }
}