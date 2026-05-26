using System;
using System.Runtime.CompilerServices;

namespace Astek.BehaviorTree
{
    /// <summary>
    /// Runs the child node if the provided condition is satisfied else fails
    /// Critical P1: This class can NEVER have more than 1 CHILD NODE
    /// Child node can be anything composite, decorator, service or task
    /// </summary>
    public class Conditional : DecoratorNode
    {
        private readonly Func<bool> _condition;

        public Conditional(string name, Func<bool> condition) : base(name)
        {
            _condition = condition;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override Status Process()
        {
            if (_condition.Invoke())
                return Children[0].Process();

            return Status.Failure;
        }
    }
}