using System;
using System.Runtime.CompilerServices;

namespace Astek.BehaviorTree
{
    [Serializable]
    public class BehaviourTree
    {
        public readonly string Name;
        public readonly RootNode Root;
        public Node.Status CurrentStatus { get; protected set; } = Node.Status.Success;

        public BehaviourTree()
        {
            Name = "Behaviour Tree";
            Root = new RootNode();
        }

        public BehaviourTree(string name)
        {
            Name = name;
            Root = new RootNode();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Tick() => CurrentStatus = Root.Process();

        public void AddChild(Node child) => Root.AddChild(child);
    }
}