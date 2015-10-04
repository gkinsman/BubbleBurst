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
            Locations = new HashSet<Point>();
        }

        public HashSet<Point> Locations { get; set; }
        public Bubble Colour { get; set; }

        public int Score => Locations.Count*(Locations.Count - 1);

        public bool Equals(BubbleGroup other)
        {
            return Colour == other.Colour && Locations.SequenceEqual(other.Locations);
        }

        public override bool Equals(object obj)
        {
            var other = obj as BubbleGroup;
            return other != null && Equals(other);
        }

        public override int GetHashCode()
        {
            return Locations.GetHashCode() ^ Colour.GetHashCode();
        }
    }
}