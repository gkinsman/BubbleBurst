﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace BubbleBurst.Bot
{
    public class LazyGeneratedTree<T> : SimpleTreeNode<T> where T : IComparable<T>
    {
        public LazyGeneratedTree(T root, Func<T, IEnumerable<T>> generator) : base(root)
        {
            this.ChildGenerator = generator;
        }

        public Func<T, IEnumerable<T>> ChildGenerator { get; private set; }

        public void SetStrategy(Func<T, IEnumerable<T>> strategy)
        {
            this.ChildGenerator = strategy;
        }
            
        public override SimpleTreeNodeList<T> Children
        {
            get
            {
                return
                    new SimpleTreeNodeList<T>(
                        ChildGenerator(this.Value)
                            .Select(x => new LazyGeneratedTree<T>(x, ChildGenerator)));
            }
        }
    }
}