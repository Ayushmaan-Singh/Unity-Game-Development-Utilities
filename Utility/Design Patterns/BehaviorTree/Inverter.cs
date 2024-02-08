using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AstekUtility.BehaviorTree
{
    public class Inverter : Node
    {
        public Inverter(string name)
        {
            Name = name;
        }

        public override Status Process()
        {
            Status childStatus = Children[0].Process();
            if (childStatus == Status.Running)
                return Status.Running;
            if (childStatus == Status.Failure)
                return Status.Success;
            else
                return Status.Failure;
        }
    }
}
