using UnityEngine;
namespace Astek.DesignPattern.HSM
{
    public abstract class State
    {
        public readonly StateMachine Machine;
        public readonly State Parent;
        public State ActiveChild;

        private bool _ready = false;

        public State(StateMachine machine, State parent)
        {
            Machine = machine;
            Parent = parent;
        }

        protected virtual void OnEnter() { }
        protected virtual void OnUpdate(float deltaTime) { }
        protected virtual void OnExit() { }

        internal void Enter()
        {
            if (Parent != null)
                Parent.ActiveChild = this;
            OnEnter();
            _ready = true;
        }
        internal void Update(float deltaTime)
        {
            if (!_ready)
                return;
            
            State transition = GetTransition();
            if (transition != null)
            {
                Machine.ChangeState(this, transition);
                return;
            }
            OnUpdate(deltaTime);
        }
        internal void Exit()
        {
            _ready = false;
            OnExit();
        }

        protected virtual State GetTransition() => null;
    }
}