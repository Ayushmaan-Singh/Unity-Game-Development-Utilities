using System.Runtime.CompilerServices;

namespace Astek.BehaviorTree
{
    public class RSelector : Node
    {
        //this bool is for making the AI decisive ,keep it false for a more unique behavior
        private bool _shuffleOnce;
        private readonly Utils _utils = new Utils();

        public RSelector(string name, bool shuffleOnce = false) : base(name)
        {
            _shuffleOnce = shuffleOnce;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override Status Process()
        {
            if (_shuffleOnce)
            {
                Children = _utils.Shuffle(Children);
                _shuffleOnce = true;
            }
            else
            {
                Children = _utils.Shuffle(Children);
            }

            Status childStatus = Children[_currentChild].Process();

            if (childStatus == Status.Running)
                return Status.Running;
            if (childStatus == Status.Success)
            {
                _currentChild = 0;
                _shuffleOnce = false;
                return Status.Success;
            }

            _currentChild++;

            if (_currentChild >= Children.Length)
            {
                _currentChild = 0;
                _shuffleOnce = false;
                return Status.Failure;
            }

            return Status.Running;
        }
    }
}