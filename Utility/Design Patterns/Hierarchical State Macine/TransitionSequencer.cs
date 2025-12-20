using System;
using System.Collections.Generic;
using System.Threading;
namespace AstekUtility.DesignPattern.HSM
{
    public class TransitionSequencer
    {
        public readonly StateMachine Machine;

        private Sequences.ISequence _sequencer;  // Current Phase (deactivate or activate)
        private Action nextPhase;                //switch structure between phases
        private (State from, State to)? pending; //coalesce a single pending request
        State lastFrom, lastTo;

        private CancellationTokenSource _cts;
        public readonly bool UseSequential = true; //Set false to use parallel

        public TransitionSequencer(StateMachine machine)
        {
            Machine = machine;
        }

        /// <summary>
        /// Request transition from one state to another
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        public void RequestTransition(State from, State to)
        {
            if (to == null || to == from) return;
            if (_sequencer != null)
            {
                pending = (from, to);
                return;
            }
            BeginTransition(from, to);
        }

        static List<Sequences.PhaseStep> GatherPhaseSteps(List<State> chain, bool deactivate)
        {
            List<Sequences.PhaseStep> steps = new List<Sequences.PhaseStep>();

            for (int i = 0; i < chain.Count; i++)
            {
                IReadOnlyList<IActivity> activity = chain[i].Activities;
                for (int j = 0; j < activity.Count; j++)
                {
                    IActivity a = activity[j];
                    if (deactivate)
                    {
                        if (a.Mode == ActivityMode.Active) steps.Add(ct => a.DeactivateAsync(ct));
                    }
                    else
                    {
                        if (a.Mode == ActivityMode.Inactive) steps.Add(ct => a.ActivateAsync(ct));
                    }
                }
            }

            return steps;
        }

        //States to exit: from->..... up to (but excluding) lca; bottom->up order
        private static List<State> StatesToExit(State from, State lca)
        {
            List<State> list = new List<State>();
            for (State s = from; s != null && s != lca; s = s.Parent) list.Add(s);
            return list;
        }

        //States to enter: path from 'to' up to (but excluding) lca; returned in entered order (top-down).
        private static List<State> StatesToEnter(State to, State lca)
        {
            Stack<State> stack = new Stack<State>();
            for (State s = to; s != lca; s = s.Parent) stack.Push(s);
            return new List<State>(stack);
        }

        private void BeginTransition(State from, State to)
        {
            _cts?.Cancel();
            _cts = new CancellationTokenSource();

            State lca = Lca(from, to);
            List<State> exitChain = StatesToExit(from, lca);
            List<State> enterChain = StatesToEnter(to, lca);

            //1. Deactivate the "old branch"
            List<Sequences.PhaseStep> exitSteps = GatherPhaseSteps(exitChain, deactivate: true);
            _sequencer = UseSequential
                ? new Sequences.SequentialPhase(exitSteps, _cts.Token)
                : new Sequences.ParallelPhase(exitSteps, _cts.Token);
            _sequencer.Start();

            nextPhase = () =>
            {
                //2. ChangeState
                Machine.ChangeState(from, to);
                //3.Activate the "new branch"
                List<Sequences.PhaseStep> enterSteps = GatherPhaseSteps(enterChain, deactivate: false);
                _sequencer = UseSequential
                    ? new Sequences.SequentialPhase(exitSteps, _cts.Token)
                    : new Sequences.ParallelPhase(exitSteps, _cts.Token);
                _sequencer.Start();
            };
        }
        private void EndTransition()
        {
            _sequencer = null;

            //Check if there was a request made while this was in progress
            if (pending.HasValue)
            {
                (State from, State to) p = pending.Value;
                pending = null;
                BeginTransition(p.from, p.to);
            }
        }

        public void Tick(float deltaTime)
        {
            if (_sequencer != null)
            {
                if (_sequencer.Update())
                {
                    if (nextPhase != null)
                    {
                        Action n = nextPhase;
                        nextPhase = null;
                        n.Invoke();
                    }
                    else
                    {
                        EndTransition();
                    }
                }
                return; //while transitioning, we don't run normal updates
            }
            Machine.InternalTick(deltaTime);
        }

        /// <summary>
        /// Compute the Lowest Common Ancestor of two states.
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static State Lca(State a, State b)
        {
            //Create a set of all parents of 'a'
            HashSet<State> ap = new HashSet<State>();
            for (State state = a; state != null; state = state.Parent) ap.Add(state);

            //Find the first parent of 'b' that is also a parent of 'a'
            for (State state = b; state != null; state = state.Parent)
                if (ap.Contains(state))
                    return state;

            //No common ancestor
            return null;
        }
    }
}