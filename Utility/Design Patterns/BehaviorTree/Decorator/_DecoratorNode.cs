using System;
using System.Runtime.CompilerServices;

namespace Astek.BehaviorTree
{
    /// <summary>
    /// These nodes can have only 1 child node
    /// </summary>
    public abstract class DecoratorNode : Node
    {
        public DecoratorNode(string name) : base(name)
        {
            Children = new Node[1];
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public new void AddChild(Node n)
        {
            if (Children.Length > 0)
            {
                try
                {
                    throw new Exception($"Conditional Node {Name}: Value overriden by new Value");
                }
                catch { }
            }

            Children[0] = n;
        }
    }
}