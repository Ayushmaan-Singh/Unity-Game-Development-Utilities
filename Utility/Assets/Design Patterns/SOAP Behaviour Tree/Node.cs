using System;
using System.Runtime.CompilerServices;
using Astek.SOAP;
using UnityEngine;

namespace Astek.DesignPattern.SOAPBehaviorTree
{
    public abstract class Node : RuntimeScriptableObject
    {
        public enum Status
        {
            Running,
            Success,
            Failure
        }

        [Flags]
        public enum UpdateMode
        {
            Frame,
            Physics,
            Late
        }

        public Status CurrentStatus { get; protected set; }
        public UpdateMode NodeUpdateMode { get; protected set; }

        public Node[] Children { get; protected set; }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public abstract Status Process(UpdateMode mode);

        protected override void OnReset()
        {
            throw new System.NotImplementedException();
        }

        public void Reset() => OnReset();
    }
}