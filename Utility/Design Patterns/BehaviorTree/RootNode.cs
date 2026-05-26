using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace Astek.BehaviorTree
{
    public class RootNode : Node
    {
        public RootNode() : base()
        {
            Name = "Root";
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override Status Process()
        {
            if (Children.Length == 0)
                return Status.Success;

            return Children[_currentChild].Process();
        }


#if UNITY_EDITOR
        private struct NodeLevel
        {
            public int level;
            public Node node;
        }

        [Tooltip("Only For Debug Purpose")]
        public void PrintTree()
        {
            string printTree = "";
            Stack<NodeLevel> nodeStack = new Stack<NodeLevel>();
            Node currentNode = this;
            nodeStack.Push(new NodeLevel
            {
                level = 0,
                node = currentNode
            });

            while (nodeStack.Count != 0)
            {
                NodeLevel nextNode = nodeStack.Pop();
                printTree += new string('-', nextNode.level) + nextNode.node.Name + "\n";

                for (int i = nextNode.node.Children.Length - 1; i >= 0; i--)
                {
                    nodeStack.Push(new NodeLevel
                    {
                        level = nextNode.level + 1,
                        node = nextNode.node.Children[i]
                    });
                }
            }

            Debug.Log(printTree);
        }
#endif
    }
}