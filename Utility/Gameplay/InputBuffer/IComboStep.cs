namespace Astek.InputBuffer
{
    /// <summary>
    /// Contract for a single step in a ComboSequence.
    ///
    /// A step owns its own matching logic entirely.
    /// ComboSequence is a dumb loop that calls TryMatch in order —
    /// it never needs to know what kind of step it is.
    ///
    /// To add a new pattern (HoldBetween, Release, Timeout, AnyOf, etc.):
    ///   1. Create a new class implementing IComboStep&lt;T&gt;
    ///   2. Add a factory to ComboStep&lt;T&gt; (optional but keeps call sites clean)
    ///   3. Done — zero changes to InputBufferQuery or any existing step.
    ///
    /// ── Cursor semantics ──────────────────────────────────────────────────────
    /// cursor is an AGE value: 0 = most recent frame, high = oldest.
    /// TryMatch receives the current cursor and must:
    ///   - Return false if the pattern cannot be satisfied within [cursor .. 0].
    ///   - Return true and set newCursor to the age just AFTER the matched region,
    ///     i.e. one step newer than the last consumed frame.
    ///     (newCursor = matchEndAge - 1, or -1 if the step consumed to age 0.)
    /// The next step receives newCursor as its starting cursor.
    /// </summary>
    public interface IComboStep<T> where T : IInputSnapshot
    {
        /// <summary>
        /// Try to match this step starting from <paramref name="cursor"/> (inclusive)
        /// and searching toward age 0.
        /// </summary>
        /// <param name="buffer">The full input history buffer.</param>
        /// <param name="cursor">Age to start searching from (oldest allowed frame).</param>
        /// <param name="newCursor">
        /// On success: age just past the matched region (next step starts here).
        /// On failure: undefined.
        /// </param>
        /// <returns>True if this step was satisfied.</returns>
        bool TryMatch(InputBuffer<T> buffer, int cursor, out int newCursor);
    }
}