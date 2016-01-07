using System.Collections.Specialized;
using System.Linq;

namespace BubbleBurst.Game
{
    public class BubbleGridStats
    {
        public BubbleGridStats(ILookup<Bubble, int> bubbleCounts)
        {
            BubbleCounts = bubbleCounts;
        }

        public ILookup<Bubble, int> BubbleCounts { get; private set; } 
    }
}