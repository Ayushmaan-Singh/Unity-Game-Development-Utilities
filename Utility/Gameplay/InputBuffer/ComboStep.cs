using System;

namespace Astek.InputBuffer
{
    /// <summary>
    /// Static factory that produces IComboStep&lt;T&gt; instances.
    /// Call sites use only this class — they never reference the concrete types directly.
    ///
    /// Adding a new pattern:
    ///   1. Implement IComboStep&lt;T&gt; in a new file (e.g. TimeoutStep.cs)
    ///   2. Add a factory method here
    ///   3. Done — InputBufferQuery and all existing steps are untouched.
    /// </summary>
    public static class ComboStep<T> where T : IInputSnapshot
    {
        // ── Core patterns ──────────────────────────────────────────────────────────

        /// <summary>
        /// One frame where predicate is true. Gaps before it are allowed.
        /// Use for button presses, one-shot transitions.
        /// </summary>
        public static IComboStep<T> Press(Func<T, bool> predicate)
            => new PressStep<T>(predicate);

        /// <summary>
        /// At least <paramref name="minFrames"/> consecutive frames where predicate is true.
        /// Use for held directions, charge inputs.
        /// </summary>
        public static IComboStep<T> Hold(Func<T, bool> predicate, int minFrames)
            => new HoldStep<T>(predicate, minFrames);

        /// <summary>
        /// A consecutive hold run whose length falls within [<paramref name="minFrames"/>, <paramref name="maxFrames"/>].
        /// The frame immediately before the run must NOT match — enforces an "exact tap" window.
        ///
        ///   // Tap held between 5 and 15 frames (short hold, not a full charge)
        ///   ComboStep&lt;T&gt;.HoldBetween(s => s.HeavyHeld, 5, 15)
        /// </summary>
        public static IComboStep<T> HoldBetween(Func<T, bool> predicate, int minFrames, int maxFrames)
            => new HoldBetweenStep<T>(predicate, minFrames, maxFrames);

        /// <summary>
        /// Predicate must be true on every frame from the current search position to age 0.
        /// Verifies the player is STILL holding at query time.
        /// Use as the LAST step in a sequence.
        /// </summary>
        public static IComboStep<T> HoldAfter(Func<T, bool> predicate, int minFrames = 1)
            => new HoldAfterStep<T>(predicate, minFrames);

        /// <summary>
        /// Finds a held → released transition (predicate true then false on the next frame).
        /// Use for tap-release timing, on-release triggers.
        /// </summary>
        public static IComboStep<T> Release(Func<T, bool> predicate)
            => new ReleaseStep<T>(predicate);

        // ── Combinators ───────────────────────────────────────────────────────────

        /// <summary>
        /// Logical OR — satisfied when any of the inner steps matches.
        /// First match wins; its cursor advancement is used.
        ///
        ///   ComboStep&lt;T&gt;.AnyOf(
        ///       ComboStep&lt;T&gt;.Press(s => s.LightPunchPressed),
        ///       ComboStep&lt;T&gt;.Press(s => s.HeavyPunchPressed)
        ///   )
        /// </summary>
        public static IComboStep<T> AnyOf(params IComboStep<T>[] steps)
            => new AnyOfStep<T>(steps);

        /// <summary>
        /// Logical NOT — satisfied when the inner step FAILS at the current cursor frame.
        /// Advances one frame on success.
        ///
        ///   ComboStep&lt;T&gt;.Not(ComboStep&lt;T&gt;.Press(s => s.IsForward))
        /// </summary>
        public static IComboStep<T> Not(IComboStep<T> inner)
            => new NegateStep<T>(inner);
    }
}