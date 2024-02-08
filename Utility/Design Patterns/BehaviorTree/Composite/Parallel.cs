using System.Collections.Generic;
using UnityEngine;

namespace AstekUtility.BehaviorTree
{
    public class Parallel : Node
    {
        /// <summary>
        /// Defines the condition for this node to end
        /// OneSucceed => Atleast one child succeed
        /// AllSucceed => All child must succeed
        /// </summary>
        public enum Policy
        {
            OneSucceed = 0,
            AllSucceed = 1,
        }

        public Policy NodePolicy { get; private set; }

        public Parallel(string name, Policy policy = Policy.OneSucceed) : base(name)
        {
            this.NodePolicy = policy;
        }

        public override Status Process()
        {
            Status[] childStatus = new Status[Children.Count];

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
                        else if (childStatus[i] == Status.Success)
                        {
                            _currentChild = 0;
                            return Status.Success;
                        }
                    }

                    break;

                case Policy.AllSucceed:

                    Status currentStatus = Status.Running;
                    for (int i = 0; i < childCount; i++)
                    {
                        if (childStatus[i] == Status.Running)
                        {
                            currentStatus = Status.Running;
                            break;
                        }
                        else if (childStatus[i] == Status.Success)
                        {
                            currentStatus = Status.Success;
                        }
                        else
                        {
                            //If even one fails then all succeed can never reach
                            return Status.Failure;
                        }
                    }
                    if (currentStatus == Status.Success)
                    {
                        _currentChild = 0;
                        return Status.Success;
                    }

                    break;

                default:
                    Debug.LogError($"Parallel Node {Name}: Invalid Policy");
                    return Status.Failure;
            }

            return Status.Failure;
        }
    }
}
