using UnityEngine;
using UnityEngine.InputSystem;


namespace Astek.InputSystem.SwipeV1
{
#if UNITY_EDITOR
    [UnityEditor.InitializeOnLoad]
#endif
    public class SwipeV1Interaction : IInputInteraction<Vector2>
    {
        private enum State
        {
            Idle,
            Started,
            Suspended
        }

        private State state = State.Idle;

#if UNITY_EDITOR
        static SwipeV1Interaction() => Initialize();
#endif

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        static void Initialize()
        {
            UnityEngine.InputSystem.InputSystem.RegisterInteraction<SwipeV1Interaction>();
        }

        public void Process(ref InputInteractionContext context)
        {
            // Default press point is 0.5, so this is true for both
            // Tracking (1) and Fired (2) — i.e. "pressed at all."
            var pressedAtAll = context.ControlIsActuated();

            // Custom threshold 1.5 is only crossed by magnitude 2 (Fired).
            // This is the trick that lets a bool query distinguish the
            // composite's third state without a dedicated API for it.
            var firedNow = context.ControlIsActuated(1.5f);

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
    }
}