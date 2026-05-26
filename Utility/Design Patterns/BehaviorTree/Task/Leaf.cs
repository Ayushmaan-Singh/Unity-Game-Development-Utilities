using System;
using System.Runtime.CompilerServices;

namespace Astek.BehaviorTree
{
    public class Leaf : Node
    {
        public delegate Status Tick();
        public event Tick ProcessMethod;

        public Leaf() : base()
        {
            Children = Array.Empty<Node>();
        }

        public Leaf(string name, Tick pm) : base(name)
        {
            ProcessMethod = pm ?? throw new ArgumentNullException(nameof(pm));
            Children = Array.Empty<Node>();
        }

        public Leaf(string name, Tick pm, int order) : base(name)
        {
            Name = name;
            SortOrder = order;
            ProcessMethod = pm ?? throw new ArgumentNullException(nameof(pm));
            Children = Array.Empty<Node>();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public new void AddChild(Node n)
        {
            throw new Exception("Cannot add a child to a leaf");
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override Status Process() => ProcessMethod?.Invoke() ?? Status.Success;
    }
}