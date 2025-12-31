using System.Collections.Generic;

//Inspiration From:
//Github => https://github.com/adammyhre/Unity-Hierarchical-StateMachine
//Youtube => https://www.youtube.com/watch?v=c-XoTg6Fba4

namespace AstekUtility.DesignPattern.HSM_Sequencer
{
    public class StateMachine
    {
        public readonly State Root;
        public readonly TransitionSequencer Sequencer;
        private bool _started;

        public StateMachine(State root)
        {
            Root = root;
            Sequencer = new TransitionSequencer(this);
        }

        public void Start()
        {
            if (_started) return;

            _started = true;
            Root.Enter();
        }

        public void Tick(float deltaTime)
        {
            if (!_started) Start();
            Sequencer.Tick(deltaTime);
        }

        internal void InternalTick(float deltaTime) => Root.Update(deltaTime);

        /// <summary>
        /// Perform the actual switch from 'from' to 'to' by exiting up to the shared ancestor, then entering down to the target
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        public void ChangeState(State from, State to)
        {
            if (from == to || from == null || to == null) return;

            State lca = TransitionSequencer.Lca(from, to);

            //Exit current branch up to (but not including) LCA
            for (State state = from; state != lca; state = state.Parent)
                state.Exit();

            //Enter target branch from LCA down to target
            Stack<State> stack = new Stack<State>();
            for (State state = to; state != lca; state = state.Parent) stack.Push(state);

            while (stack.Count > 0) stack.Pop().Enter();
        }
    }
}