using System;
using UnityEngine;

namespace Astek.BehaviorTree
{
    public class Parallel : Node
    {
        /// <summary>
        /// Defines the condition for this node to end
        /// OneSucceed => At least one child succeed
        /// AllSucceed => All child must succeed
        /// </summary>
        public enum Policy
        {
            OneSucceed = 0,
            AllSucceed = 1
        }

        public Parallel(string name, Policy policy = Policy.OneSucceed) : base(name)
        {
            NodePolicy = policy;
        }

        public Policy NodePolicy { get; } = Policy.OneSucceed;

        public override Status Process()
        {
            Status[] childStatus = new Status[Children.Length];

            int childCount = childStatus.Length;

            for (int i = 0; i < childCount; i++)
            {
                childStatus[i] = Children[i].Process();
            }

            switch (NodePolicy)
            {
                case Policy.OneSucceed:

                    for (int i = 0; i < childCount; i++)
                    {
                        if (childStatus[i] == Status.Running)
                            return Status.Running;
                        if (childStatus[i] == Status.Success)
                        {
                            _currentChild = 0;
                            return Status.Success;
                        }
                    }

                    break;

                case Policy.AllSucceed:

                    Status currentStatus = Status.Success;
                    for (int i = 0; i < childCount; i++)
                        currentStatus |= childStatus[i];


                    if((currentStatus & Status.Running) == Status.Running)
                        return  Status.Running;
                    if (currentStatus == Status.Success)
                    {
                        _currentChild = 0;
                        return Status.Success;
                    }
                    if(currentStatus == Status.Failure)
                        return Status.Failure;

                    break;

                default:
                    throw new Exception($"Parallel Node {Name}: Invalid Policy");
            }

            return Status.Failure;
        }
    }
}