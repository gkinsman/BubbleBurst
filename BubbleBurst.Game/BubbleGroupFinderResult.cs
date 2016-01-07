using System.Collections.Generic;

namespace BubbleBurst.Game
{
    public class BubbleGroupFinderResult
    {
        public BubbleGroupFinderResult(IEnumerable<BubbleGroup> groups, Dictionary<Bubble, int> statistics)
        {
            Groups = groups;
            Statistics = statistics;
        }

        public IEnumerable<BubbleGroup> Groups { get; set; }

        public Dictionary<Bubble, int> Statistics { get; set; }
    }
}