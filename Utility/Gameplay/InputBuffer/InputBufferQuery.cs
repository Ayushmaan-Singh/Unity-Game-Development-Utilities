using System;

namespace Astek.InputBuffer
{
    /// <summary>
    /// Fluent query object returned by InputBuffer.Query(windowFrames).
    /// Operates over a read-only window of the last N frames, oldest → newest.
    ///
    /// Three query modes:
    ///
    ///   Any()             — at least one frame in the window satisfies a predicate.
    ///                       Use for: coyote time, jump buffer, single-press detection.
    ///
    ///   Sequence()        — steps appear in order, anywhere in the window,
    ///                       with arbitrary gaps allowed between them.
    ///                       Use for: QCF motions, charge → release, any lenient combo.
    ///
    ///   StrictSequence()  — steps appear in order in consecutive frame slots.
    ///                       No gaps. Each step occupies exactly one frame.
    ///                       Use for: double-tap, rapid jab-jab, tick-perfect inputs.
    ///
    /// All methods iterate oldest → newest (PeekAt(window-1) … PeekAt(0))
    /// so "sequence" reads naturally left-to-right in time.
    /// </summary>
    public readonly struct InputBufferQuery<T> where T : IInputSnapshot
    {
        private readonly InputBuffer<T> _buffer;
        private readonly int _window; // number of frames to search

        internal InputBufferQuery(InputBuffer<T> buffer, int window)
        {
            _buffer = buffer;
            _window = window;
        }

        // ── Any ───────────────────────────────────────────────────────────────────

        /// <summary>
        /// True if at least one frame in the window satisfies <paramref name="predicate"/>.
        ///
        ///   // Coyote time (6 frames)
        ///   buffer.Query(6).Any(s => s.OnGround)
        ///
        ///   // Jump buffer, ignoring already-consumed presses
        ///   buffer.Query(4).Any(s => s.JumpPressed && s.Frame > lastJumpConsumedFrame)
        /// </summary>
        public bool Any(Func<T, bool> predicate)
        {
            for (int age = _window - 1; age >= 0; age--)
                if (predicate(_buffer.PeekAt(age)))
                    return true;
            return false;
        }

        /// <summary>
        /// Returns the most recent frame satisfying <paramref name="predicate"/>,
        /// or default if none found. Useful for reading the frame index to mark consumption.
        ///
        ///   var hit = buffer.Query(4).First(s => s.JumpPressed);
        ///   if (hit.Frame > lastConsumedFrame) { lastConsumedFrame = buffer.CurrentFrame; }
        /// </summary>
        public T First(Func<T, bool> predicate)
        {
            // Search newest → oldest so First() returns the most recent match
            for (int age = 0; age < _window; age++)
            {
                T snap = _buffer.PeekAt(age);
                if (predicate(snap)) return snap;
            }
            return default;
        }

        // ── Sequence ──────────────────────────────────────────────────────────────

        /// <summary>
        /// True if all steps appear in order anywhere in the window.
        /// Gaps between steps are allowed — only ordering is enforced.
        /// Uses a greedy oldest-to-newest scan: finds step[0], then looks
        /// forward for step[1], and so on.
        ///
        ///   // Quarter-circle forward + punch in 30 frames
        ///   buffer.Query(30).Sequence(
        ///       s => s.IsDown,
        ///       s => s.IsDownForward,
        ///       s => s.IsForward,
        ///       s => s.PunchPressed
        ///   )
        ///
        ///   // Charge → release (hold back 20+ frames then press forward)
        ///   buffer.Query(30).Sequence(
        ///       s => s.IsBack,
        ///       s => s.IsForward && s.PunchPressed
        ///   )
        /// </summary>
        public bool Sequence(params Func<T, bool>[] steps)
        {
            if (steps == null || steps.Length == 0) return true;

            int stepIdx = 0;

            // Walk oldest → newest
            for (int age = _window - 1; age >= 0 && stepIdx < steps.Length; age--)
            {
                if (steps[stepIdx](_buffer.PeekAt(age)))
                    stepIdx++;
            }

            return stepIdx == steps.Length;
        }

        // ── StrictSequence ────────────────────────────────────────────────────────

        /// <summary>
        /// True if steps appear in consecutive frames somewhere in the window.
        /// No gaps — each step occupies exactly one frame slot.
        /// Slides a window of (steps.Length) frames across the buffer.
        ///
        ///   // Double-tap right (no gap tolerated)
        ///   buffer.Query(10).StrictSequence(
        ///       s => s.RightPressed,
        ///       s => !s.RightPressed,    // release frame
        ///       s => s.RightPressed
        ///   )
        ///
        ///   // Jab-jab link — two presses exactly 1 frame apart
        ///   buffer.Query(6).StrictSequence(
        ///       s => s.JabPressed,
        ///       s => s.JabPressed
        ///   )
        /// </summary>
        public bool StrictSequence(params Func<T, bool>[] steps)
        {
            if (steps == null || steps.Length == 0) return true;
            if (steps.Length > _window) return false;

            int frameCount = _window;
            int stepCount = steps.Length;

            // Slide a window of stepCount frames, oldest → newest
            // Outer loop: starting age of the window (oldest frame of the candidate)
            for (int startAge = frameCount - 1; startAge >= stepCount - 1; startAge--)
            {
                bool match = true;
                for (int i = 0; i < stepCount; i++)
                {
                    // age decreases as i increases → walking forward in time
                    int age = startAge - i;
                    if (!steps[i](_buffer.PeekAt(age)))
                    {
                        match = false;
                        break;
                    }
                }
                if (match) return true;
            }

            return false;
        }

        /// <summary>
        /// How many frames this query covers (after clamping to buffer.Count).
        /// </summary>
        public int Window => _window;

        // ── ComboSequence ─────────────────────────────────────────────────────────

        /// <summary>
        /// Evaluates a sequence of IComboStep&lt;T&gt; steps in order, oldest → newest.
        ///
        /// Each step owns its own matching logic entirely — this method is a dumb loop.
        /// Adding new step types (HoldBetween, Release, Timeout, AnyOf…) requires
        /// zero changes here: just implement IComboStep&lt;T&gt; and pass an instance in.
        ///
        ///   // Hold down 20f → tap attack → hold heavy right now
        ///   buffer.Query(60).ComboSequence(
        ///       ComboStep&lt;PS&gt;.Hold     (s => s.InputY &lt; 0,   20),
        ///       ComboStep&lt;PS&gt;.Press    (s => s.AttackPressed    ),
        ///       ComboStep&lt;PS&gt;.HoldAfter(s => s.HeavyHeld,    1 )
        ///   )
        ///
        ///   // Short hold (5–15 frames) then press — "tap charge" pattern
        ///   buffer.Query(30).ComboSequence(
        ///       ComboStep&lt;FS&gt;.HoldBetween(s => s.IsBack,      5, 15),
        ///       ComboStep&lt;FS&gt;.Press      (s => s.AnyPunchPressed    )
        ///   )
        /// </summary>
        public bool ComboSequence(params IComboStep<T>[] steps)
        {
            if (steps == null || steps.Length == 0) return true;

            int cursor = _window - 1; // start at the oldest frame in the window

            foreach (var step in steps)
            {
                if (!step.TryMatch(_buffer, cursor, out cursor))
                    return false;

                // cursor < 0 means a step consumed all remaining frames (e.g. HoldAfter)
                // If more steps follow they will immediately fail — correct behaviour.
                if (cursor < 0) break;
            }

            return true;
        }

        // ── HeldFor helper ────────────────────────────────────────────────────────

        /// <summary>
        /// True if predicate was true for at least <paramref name="minFrames"/>
        /// consecutive frames anywhere in the window.
        ///
        /// Delegates to HoldStep so the logic lives in one place.
        ///
        ///   buffer.Query(30).HeldFor(s => s.IsDown, 20)
        /// </summary>
        public bool HeldFor(Func<T, bool> predicate, int minFrames)
        {
            var step = new HoldStep<T>(predicate, minFrames);
            return step.TryMatch(_buffer, _window - 1, out _);
        }

        /// <summary>
        /// True if a consecutive hold run exists with length between
        /// <paramref name="minFrames"/> and <paramref name="maxFrames"/> inclusive.
        ///
        ///   buffer.Query(60).HeldBetween(s => s.HeavyHeld, 5, 20)
        /// </summary>
        public bool HeldBetween(Func<T, bool> predicate, int minFrames, int maxFrames)
        {
            var step = new HoldBetweenStep<T>(predicate, minFrames, maxFrames);
            return step.TryMatch(_buffer, _window - 1, out _);
        }
    }
}