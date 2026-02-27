namespace Astek.BehaviorTree
{
    public class PSelector : Node
    {
        public PSelector(string name) : base(name) { }

        private void OrderNodes()
        {
            Children = Children.OrderBy(x => x.SortOrder).ToArray();
        }

        public override Status Process()
        {
            OrderNodes();

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