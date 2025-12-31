using System.Collections.Generic;

namespace AstekUtility.DesignPattern.HSM
{
    public class StateMachine
    {
        public State Root { get; private set; }
        public State CurrentState { get; private set; }
        public State PreviousState { get; private set; }

        private bool _started = false;

        public StateMachine(State root)
        {
            Root = root;
        }
        
        public void Start()
        {
            _started = true;
            CurrentState = Root;
            CurrentState.Enter();
        }
        public void Update(float deltaTime)
        {
            if (!_started) Start();
            CurrentState.Update(deltaTime);
        }

        public void ChangeState(State from, State to)
        {
            if (from == to || from == null || to == null) return;

            State lca = LCA(from, to);

            // Exit current branch up to (but not including) LCA
            for (State s = from; s != lca; s = s.Parent) s.Exit();

            // Enter target branch from LCA down to target
            Stack<State> stack = new Stack<State>();
            for (State s = to; s != lca; s = s.Parent) stack.Push(s);
            while (stack.Count > 0) stack.Pop().Enter();
            PreviousState = CurrentState;
            CurrentState = to;
        }
        //Lowest common ancestor
        private State LCA(State a, State b)
        {
            HashSet<State> ap = new HashSet<State>();
            for (State s = a; s != null; s = s.Parent) ap.Add(s);

            for (State s = b; s != null; s = s.Parent)
            {
                if (ap.Contains(s))
                    return s;
            }

            return null;
        }
    }
}