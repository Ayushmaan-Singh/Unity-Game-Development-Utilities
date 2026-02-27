using System;

namespace Astek.BehaviorTree
{
    public class Cooldown : Node
    {
        private readonly float _cooldown;
        private double _timeCounter;
        private event Func<float> _tickTime = () => 0;

        public Cooldown(string name, float cooldown, Func<float> timeTick) : base(name)
        {
            _cooldown = cooldown;
            _tickTime += timeTick;
            Children = new Node[1];
            _timeCounter = 0;
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
            _timeCounter += _tickTime.Invoke();
            if (_timeCounter >= _cooldown)
                return Children[0].Process();

            return Status.Running;
        }
    }
}