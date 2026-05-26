using System;
using System.Runtime.CompilerServices;

namespace Astek.BehaviorTree
{
    public class Cooldown : DecoratorNode
    {
        private readonly float _cooldown;
        private double _timeCounter;
        private event Func<float> _tickTime = () => 0;

        public Cooldown(string name, float cooldown, Func<float> timeTick) : base(name)
        {
            _cooldown = cooldown;
            _tickTime += timeTick;
            _timeCounter = 0;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override Status Process()
        {
            _timeCounter += _tickTime.Invoke();
            if (_timeCounter >= _cooldown)
                return Children[0].Process();

            return Status.Running;
        }
    }
}