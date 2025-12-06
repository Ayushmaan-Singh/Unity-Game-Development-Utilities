using System.Collections.Generic;
using UnityEngine;

namespace AstekUtility.DesignPattern.HSM
{
    public abstract class State
    {
        public readonly StateMachine Machine;
        public readonly State Parent;
        public State ActiveChild;
        private readonly List<IActivity> _activities = new List<IActivity>();
        public IReadOnlyList<IActivity> Activities => _activities;

        public State(StateMachine machine, State parent = null)
        {
            Machine = machine;
            Parent = parent;
        }

        public void Add(IActivity a)
        {
            if (a != null)
                _activities.Add(a);
        }

        /// <summary>
        /// Set initial child to enter when this state starts default value is null(null = this is the leaf)
        /// </summary>
        /// <returns></returns>
        protected virtual State GetInitialState() => null;

        /// <summary>
        /// Target state to switch to this frame (null=stay in current state)
        /// </summary>
        /// <returns></returns>
        protected virtual State GetTransition() => null;

        //Lifecycle hooks
        protected virtual void OnEnter() { }
        protected virtual void OnExit() { }
        protected virtual void OnUpdate(float deltaTime) { }

        internal void Enter()
        {
            if (Parent != null)
                Parent.ActiveChild = this;

            OnEnter();
            State init = GetInitialState();
            if (init != null) init.Enter();
        }
        internal void Exit()
        {
            if (ActiveChild != null)
                ActiveChild.Exit();

            ActiveChild = null;
            OnExit();
        }
        internal void Update(float deltaTime)
        {
            State transition = GetTransition();
            if (transition != null)
            {
                Machine.Sequencer.RequestTransition(this, transition);
                return;
            }

            if (ActiveChild != null) ActiveChild.Update(deltaTime);
            OnUpdate(deltaTime);
        }

        /// <summary>
        /// Returns the deepest currently active descendant state(the leaf of the active path).
        /// </summary>
        /// <returns></returns>
        public State Leaf()
        {
            State state = this;
            while (state.ActiveChild != null) state = state.ActiveChild;
            return state;
        }

        /// <summary>
        /// Yields this state and then each ancestor up to the root (self->parent->....->root)
        /// </summary>
        /// <returns></returns>
        public IEnumerable<State> PathToRoot()
        {
            for (State state = this; state != null; state = state.Parent) yield return state;
        }
    }
}