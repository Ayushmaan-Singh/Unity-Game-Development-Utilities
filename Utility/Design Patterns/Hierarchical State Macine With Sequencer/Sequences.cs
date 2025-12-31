using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
namespace AstekUtility.DesignPattern.HSM_Sequencer
{
    public class Sequences
    {
        public interface ISequence
        {
            bool IsDone { get; }
            void Start();
            bool Update();
        }

        //One activity operation (activate OR deactivate) to run fot this phase.
        public delegate Task PhaseStep(CancellationToken token);

        public class ParallelPhase : ISequence
        {
            private readonly List<PhaseStep> _steps;
            readonly CancellationToken _ct;
            private List<Task> _tasks;
            public bool IsDone { get; private set; }

            public ParallelPhase(List<PhaseStep> steps, CancellationToken ct)
            {
                _steps = steps;
                _ct = ct;
            }

            public void Start()
            {
                if (_steps == null || _steps.Count == 0)
                {
                    IsDone = true;
                    return;
                }
                _tasks = new List<Task>(_steps.Count);
                for (int i = 0; i < _steps.Count; i++)
                    _tasks.Add(_steps[i].Invoke(_ct));
            }
            public bool Update()
            {
                if (IsDone) return true;
                IsDone=_tasks==null || _tasks.TrueForAll(t=>t.IsCompleted);
                return IsDone;
            }
        }
        public class SequentialPhase : ISequence
        {
            private readonly List<PhaseStep> _steps;
            readonly CancellationToken _ct;
            private int _index = -1;
            private Task _current;
            public bool IsDone { get; private set; }

            public SequentialPhase(List<PhaseStep> steps, CancellationToken ct)
            {
                _steps = steps;
                _ct = ct;
            }

            public void Start() => Next();
            private void Next()
            {
                _index++;
                if (_index >= _steps.Count)
                {
                    IsDone = true;
                    return;
                }
                _current = _steps[_index].Invoke(_ct);
            }

            public bool Update()
            {
                if (IsDone) return true;
                if (_current == null || _current.IsCompleted) Next();
                return IsDone;
            }
        }

        /// <summary>
        /// Null Object Sequence for Sequencer Default value
        /// </summary>
        public class NoopPhase : ISequence
        {
            public bool IsDone { get; private set; }
            public void Start() => IsDone = true; //Completes immediately
            public bool Update() => IsDone;
        }
    }
}