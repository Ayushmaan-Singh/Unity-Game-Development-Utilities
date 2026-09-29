using UnityEngine;
using UnityEngine.InputSystem;


namespace Astek.InputSystem
{
#if UNITY_EDITOR
    [UnityEditor.InitializeOnLoad]
#endif
    public class SwipeV2Interaction : IInputInteraction<Vector2Int>
    {
        private enum State
        {
            Idle,
            Started,
            Suspended
        }

        private State state = State.Idle;

        public void Process(ref InputInteractionContext context)
        {
            // Default press point is 0.5, so this is true for both
            // Tracking (1) and Fired (2) — i.e. "pressed at all."
            bool pressedAtAll = context.ControlIsActuated();

            // Custom threshold 1.5 is only crossed by magnitude 2 (Fired).
            // This is the trick that lets a bool query distinguish the
            // composite's third state without a dedicated API for it.
            bool firedNow = context.ControlIsActuated(1.5f);

            if (pressedAtAll)
            {
                switch (state)
                {
                    case State.Idle:
                        state = State.Started;
                        context.Started();
                        return;

                    case State.Started:
                        if (firedNow)
                        {
                            context.Performed();
                            state = State.Suspended; // (2) guard — no re-fire
                        }
                        return;

                    case State.Suspended:
                        return; // inert until release
                }
            }
            else
            {
                if (state == State.Started)
                    context.Canceled(); // held, moved, never crossed threshold

                state = State.Idle;
            }
        }

        public void Reset()
        {
            state = State.Idle;
        }
        
#if UNITY_EDITOR
        static SwipeV2Interaction() => Initialize();
#endif

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        static void Initialize()
        {
            UnityEngine.InputSystem.InputSystem.RegisterInteraction<SwipeV2Interaction>();
        }
    }
}