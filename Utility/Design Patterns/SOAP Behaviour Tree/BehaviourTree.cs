using System.Runtime.CompilerServices;
using Astek.SOAP;
using UnityEngine;

namespace Astek.DesignPattern.SOAPBehaviorTree
{
    public class BehaviourTree : RuntimeScriptableObject
    {
        [SerializeField] private Node _rootNode;

        public Node.Status CurrentStatus { get; private set; }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Tick() => CurrentStatus = _rootNode.Process(Node.UpdateMode.Frame);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void PhysicsTick() => CurrentStatus = _rootNode.Process(Node.UpdateMode.Physics);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void LateTick() => CurrentStatus = _rootNode.Process(Node.UpdateMode.Late);

        protected override void OnReset()
        {
            _rootNode.Reset();
            CurrentStatus = Node.Status.Success;
        }

        public void Reset() => OnReset();
    }
}