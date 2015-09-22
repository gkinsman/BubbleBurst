using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace BubbleBurst.Game
{
    public class BubbleGroup : IEquatable<BubbleGroup>
    {
        public BubbleGroup()
        {
            Points = new HashSet<Point>();
        }

        public HashSet<Point> Points { get; set; }
        public Bubble Colour { get; set; }

        public int Score => Points.Count * (Points.Count - 1);

        public bool Equals(BubbleGroup other)
        {
            return Colour == other.Colour && Points.SequenceEqual(other.Points);
        }

        public override bool Equals(object obj)
        {
            var other = obj as BubbleGroup;
            return other != null && Equals(other);
        }

        public override int GetHashCode()
        {
            return Points.GetHashCode() ^ Colour.GetHashCode();
        }
    }
}
