using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.InputSystem.Utilities;
using UnityEngine.InputSystem.Layouts;

namespace Astek.InputSystem
{
#if UNITY_EDITOR
    [UnityEditor.InitializeOnLoad]
#endif
    public class SwipeV2Composite : InputBindingComposite<Vector2Int>
    {
        [InputControl(layout = "Button")]
        public int press;

        [InputControl(layout = "Vector2")]
        public int position;

        public float minDistance = 100f;
        public float minSpeed = 500f;

        private enum State
        {
            Idle,
            Tracking,
            Fired
        }

        private State state = State.Idle;
        private Vector2 startPosition;
        private Vector2 frozenDelta;
        private double startTime;

#if UNITY_EDITOR
        static SwipeV2Composite() => Initialize();
#endif

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        static void Initialize()
        {
            UnityEngine.InputSystem.InputSystem.RegisterBindingComposite<SwipeV2Composite>("SwipeV2");
        }

        // Single source of truth for state transitions. Called at the top of
        // BOTH ReadValue and EvaluateMagnitude, so neither depends on the
        // other having run first this event.
        private Vector2 UpdateState(ref InputBindingCompositeContext context)
        {
            bool pressed = context.ReadValueAsButton(press);
            Vector2 currentPos = context.ReadValue<Vector2, Vector2MagnitudeComparer>(position);

            if (pressed && state == State.Idle)
            {
                state         = State.Tracking;
                startPosition = currentPos;
                startTime     = InputState.currentTime;
            }
            else if (!pressed)
            {
                state = State.Idle;
                return Vector2.zero;
            }

            if (state == State.Fired)
                return frozenDelta;

            Vector2 delta = currentPos - startPosition;

            if (state == State.Tracking)
            {
                var elapsed = InputState.currentTime - startTime;
                var speed = delta.magnitude / (float)System.Math.Max(elapsed, 0.0001);

                if (delta.magnitude >= minDistance && speed >= minSpeed)
                {
                    state       = State.Fired;
                    frozenDelta = delta;
                    return frozenDelta;
                }
            }

            return delta;
        }

        public override Vector2Int ReadValue(ref InputBindingCompositeContext context)
        {
            return UpdateState(ref context).EvaluateDir8();
        }

        public override float EvaluateMagnitude(ref InputBindingCompositeContext context)
        {
            UpdateState(ref context); // ensure state is current even if this runs first
            return state switch
            {
                State.Idle => 0f,
                State.Tracking => 1f,
                State.Fired => 2f,
                _ => 0f
            };
        }
    }
}