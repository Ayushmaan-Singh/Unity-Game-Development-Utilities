using System;

namespace Astek.BehaviorTree
{
    /// <summary>
    /// Runs the child node if the provided condition is satisfied else fails
    /// Critical P1: This class can NEVER have more than 1 CHILD NODE
    /// Child node can be anything composite, decorator, service or task
    /// </summary>
    public class Conditional : Node
    {
        private readonly Func<bool> _condition;
        public readonly new Node[] Children;

        public Conditional(string name, Func<bool> condition) : base(name)
        {
            _condition = condition;
            Children = new Node[1];
        }

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

        public override Status Process()
        {
            if (_condition.Invoke())
            {
                return Children[0].Process();
            }

            return Status.Failure;
        }
    }
}