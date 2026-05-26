using System;
using System.Runtime.CompilerServices;

namespace Astek.BehaviorTree
{
    /// <summary>
    /// Waits as long as provided predicate returns false
    /// Critical P1: This class can NEVER have more than 1 CHILD NODE.
    /// Child node can be anything composite, decorator, service or task
    /// </summary>
    public class WaitUntil : DecoratorNode
    {
        private readonly Func<bool> _predicate;

        public WaitUntil(string name, Func<bool> predicate) : base(name)
        {
            _predicate = predicate ?? throw new ArgumentNullException(nameof(predicate));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override Status Process()
        {
            if (_predicate.Invoke())
                return Children[0].Process();
            return Status.Running;
        }
    }
}