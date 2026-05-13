namespace Astek.InputBuffer
{
    /// <summary>
    /// The only contract InputBuffer needs from a snapshot.
    /// Every game defines its own concrete snapshot struct:
    ///
    ///   struct PlatformerSnapshot : IInputSnapshot { ... }
    ///   struct FighterSnapshot    : IInputSnapshot { ... }
    ///
    /// The buffer stamps Frame automatically inside Record().
    /// </summary>
    public interface IInputSnapshot
    {
        /// <summary>
        /// Monotonic frame index stamped by InputBuffer.Record().
        /// Use it to prevent double-consuming the same press:
        ///   buffer.Query(4).Any(s => s.JumpPressed && s.Frame > lastJumpConsumedFrame)
        /// After consuming, store buffer.CurrentFrame as lastJumpConsumedFrame.
        /// </summary>
        long Frame { get; set; }
    }
}