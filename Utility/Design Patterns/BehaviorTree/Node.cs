using System;

namespace Astek.BehaviorTree
{
    public class Node
    {
        [Flags]
        public enum Status
        {
            Failure = 0,
            Running = 1 << 0,
            Success = 1 << 1,
        }

        protected int _currentChild;

        public string Name { get; protected set; }
        public int SortOrder { get; protected set; }

        public Status NodeStatus { get; protected set; }

        //Put in order they are to be executed if it's a sequence type Node
        public Node[] Children { get; protected set; } = Array.Empty<Node>();

        public Node() { }

        public Node(string name)
        {
            Name = name;
        }

        public Node(string name, int order)
        {
            Name = name;
            SortOrder = order;
        }

        public void AddChild(Node n)
        {
            if (n == null)
                throw new ArgumentNullException(nameof(n));
            Node[] newCollection = new Node[Children.Length + 1];
            Array.Copy(Children, newCollection, Children.Length);
            newCollection[Children.Length] = n;
        }

        public virtual Status Process()
        {
            return Children[_currentChild].Process();
        }

        public void Reset()
        {
            Children.ForEach(child => child.Reset());
            _currentChild = 0;
        }
    }
}