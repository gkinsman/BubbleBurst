using System;

namespace BubbleBurst.Game.Extensions
{
    public static class BubbleExtensions
    {
        public static Bubble ToBubble(this char c)
        {
            switch (c)
            {
                case 'G':
                    return Bubble.Green;
                case 'R':
                    return Bubble.Red;
                case 'Y':
                    return Bubble.Yellow;
                case 'B':
                    return Bubble.Blue;
                case 'C':
                    return Bubble.Cyan;
                default:
                    throw new ArgumentException("Input character must be G, R, Y, B or P");
            }
        }
    }
}