using System;
using System.Runtime.CompilerServices;

namespace Astek.BehaviorTree
{
    public class Parallel : Node
    {
        private readonly bool _async;
        private readonly int _reqSuccessCount;

        /// <param name="name">Name of node</param>
        /// <param name="requiredSuccessCount">Default value is -1 which means all nodes need to succeed to return success</param>
        /// <param name="asynchronous">Default value is false which means all nodes are processed one after another, use this in case of many children</param>
        /// <param name="type">When this node should be processed Frame or physics update</param>
        public Parallel(string name, int requiredSuccessCount = -1, bool asynchronous = false) : base(name)
        {
            _async = asynchronous;
            _reqSuccessCount = requiredSuccessCount >= 0 ? Math.Clamp(requiredSuccessCount, 0, Children.Length) : -1;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public new void AddChild(Node n)
        {
            if (n == null)
                throw new ArgumentNullException(nameof(n));
            Node[] newCollection = new Node[Children.Length + 1];
            Array.Copy(Children, newCollection, Children.Length);
            newCollection[Children.Length] = n;
            Children = newCollection.OrderBy(child => child.SortOrder).ToArray();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override Status Process()
        {
            Status[] childrenStatuses = new Status[Children.Length];
            if (!_async)
                for (int i = 0; i < Children.Length; i++)
                    childrenStatuses[i] = Children[i].Process();
            else
                System.Threading.Tasks.Parallel.For(0, Children.Length, i => childrenStatuses[i] = Children[i].Process());

            int successCount = childrenStatuses.CountBy(x => x == Status.Success);
            int runningCount = childrenStatuses.CountBy(x => x == Status.Running);

            if ((_reqSuccessCount == -1 && successCount == Children.Length) ||
                (_reqSuccessCount >= 0 && successCount >= _reqSuccessCount))
                return Status.Success;
            if (runningCount == 0)
                return Status.Failure;

            return Status.Running;
        }
    }
}