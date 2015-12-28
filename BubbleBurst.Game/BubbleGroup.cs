using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.Remoting.Messaging;

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

        public bool IsValidFor(ImmutableBubbleBurstGrid grid)
        {
            return Locations.All(location => grid[location.X, location.Y] == Colour);
        }

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