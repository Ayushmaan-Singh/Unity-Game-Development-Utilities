using System.Runtime.CompilerServices;

namespace Astek.BehaviorTree
{
    public class Sequence : Node
    {
        public Sequence(string name) : base(name) { }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override Status Process()
        {
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