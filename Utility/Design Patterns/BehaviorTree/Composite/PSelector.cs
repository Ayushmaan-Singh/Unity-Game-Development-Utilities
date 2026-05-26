using System.Runtime.CompilerServices;

namespace Astek.BehaviorTree
{
    public class PSelector : Node
    {
        public PSelector(string name) : base(name) { }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override Status Process()
        {
            Status childStatus = Children[_currentChild].Process();

            if (childStatus == Status.Running)
                return Status.Running;
            if (childStatus == Status.Success)
            {
                _currentChild = 0;
                return Status.Success;
            }

            _currentChild++;

            if (_currentChild >= Children.Length)
            {
                _currentChild = 0;
                return Status.Failure;
            }

            return Status.Running;
        }
    }
}