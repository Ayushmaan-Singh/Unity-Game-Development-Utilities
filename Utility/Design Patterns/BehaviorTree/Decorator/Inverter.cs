using System.Runtime.CompilerServices;

namespace Astek.BehaviorTree
{
    public class Inverter : DecoratorNode
    {
        public Inverter(string name) : base(name) { }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override Status Process()
        {
            Status status = Children[0].Process();
            if (status == Status.Success)
                return Status.Failure;
            if (status == Status.Failure)
                return Status.Success;

            return Status.Running;
        }
    }
}