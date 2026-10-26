using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.InputSystem.Utilities;
using UnityEngine.InputSystem.Layouts;

namespace Astek.InputSystem
{
    /// <summary>
    /// Swipe composite. Outputs Vector2.zero (magnitude 0) until a swipe is recognised,
    /// then the swipe direction (magnitude 1) until the pointer is released.
    /// Use on a Button action: 'started'/'performed' fire once when the swipe is recognised.
    /// Bindings: press = &lt;Pointer&gt;/press, position = &lt;Pointer&gt;/position.
    /// </summary>
#if UNITY_EDITOR
    [UnityEditor.InitializeOnLoad]
#endif
    public class SwipeDirectionalComposite : InputBindingComposite<Vector2>
    {
        [InputControl(layout = "Button")]
        public int press;

        [InputControl(layout = "Vector2")]
        public int position;

        [Tooltip("Distance the pointer must travel inside the time window, in centimetres.")]
        public float minDistanceCm = 1.5f;

        [Tooltip("Length of the sliding time window, in milliseconds.")]
        public float windowMs = 100f;

        [Tooltip("Angle (degrees) given to each diagonal sector. 0 = 4-way, 45 = plain 8-way, ~30 favours cardinals."), Range(0f, 45f)]
        public float diagonalWidth = 30f;

        private readonly SwipeRecognizer _recognizer = new();
        private Vector2 _direction;

#if UNITY_EDITOR
        static SwipeDirectionalComposite() => Initialize();
#endif

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        static void Initialize()
        {
            // Fully qualified: inside namespace Astek.InputSystem the bare name "InputSystem"
            // resolves to our own namespace, not UnityEngine.InputSystem.InputSystem.
            UnityEngine.InputSystem.InputSystem.RegisterBindingComposite<SwipeDirectionalComposite>("Swipe Directional");
        }

        // Pulls the current part values, advances the recogniser and caches the direction.
        // Called from both ReadValue and EvaluateMagnitude; the recogniser ignores duplicate samples,
        // so calling it repeatedly for the same event is harmless.
        private void Refresh(ref InputBindingCompositeContext context)
        {
            bool pressed = context.ReadValueAsButton(press);
            Vector2 pos = context.ReadValue<Vector2, Vector2MagnitudeComparer>(position);

            float dpi = Screen.dpi > 0f ? Screen.dpi : 160f;
            _recognizer.MinDistance = minDistanceCm / 2.54f * dpi;
            _recognizer.Window      = windowMs / 1000.0;

            bool wasFired = _recognizer.Fired;
            _recognizer.Feed(pressed, pos, InputState.currentTime);

            if (!_recognizer.Fired)
                _direction = Vector2.zero;
            else if (!wasFired)
                _direction = _recognizer.Delta;
        }

        public override Vector2 ReadValue(ref InputBindingCompositeContext context)
        {
            Refresh(ref context);
            return _direction;
        }

        public override float EvaluateMagnitude(ref InputBindingCompositeContext context)
        {
            Refresh(ref context);
            return _recognizer.Fired ? 1f : 0f;
        }
    }
}