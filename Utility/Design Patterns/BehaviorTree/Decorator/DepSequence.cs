//To use dependency sequence 
//Make a behavior tree for dependecy behavior add all the behavior to this dependency tree
//Add child nodes to this node
//With above setup every frame 

using System.Runtime.CompilerServices;

namespace Astek.BehaviorTree
{
    /// <summary>
    ///     This is a dependency sequence used when we want to check a condition while a particular sequence
    /// </summary>
    public class DepSequence : DecoratorNode
    {
        private readonly RootNode _dependencyTreeTree;

        public DepSequence(string name, RootNode dependencyTree) : base(name)
        {
            Name = name;
            _dependencyTreeTree = dependencyTree;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override Status Process()
        {
            if (_dependencyTreeTree.Process() == Status.Failure)
            {
                //Reset All Children
                foreach (Node n in Children)
                    n.Reset();
                return Status.Failure;
            }

            Status childStatus = Children[_currentChild].Process();

            if (childStatus == Status.Running)
                return Status.Running;
            if (childStatus == Status.Failure)
                return Status.Failure;

            _currentChild++;
            if (_currentChild >= Children.Length)
            {
                _currentChild = 0;
                return Status.Success;
            }

            return Status.Running;
        }
    }
}