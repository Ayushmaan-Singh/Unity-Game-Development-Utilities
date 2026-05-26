namespace Astek.InputBuffer
{
#if UNITY_EDITOR

    namespace EditorOnly
    {
        /// <summary>
        /// Example snapshot for a 2D fighting game.
        /// Shows that InputBuffer/InputBufferQuery are truly game-agnostic —
        /// only the snapshot type changes.
        ///
        /// Stick directions use the numpad convention:
        ///   7 8 9
        ///   4 5 6   (5 = neutral)
        ///   1 2 3
        ///
        /// ── Usage examples ────────────────────────────────────────────────────────
        ///
        ///   // Hadouken (QCF + punch) within 30 frames, gaps allowed
        ///   buffer.Query(30).Sequence(
        ///       s => s.IsDown,
        ///       s => s.IsDownForward,
        ///       s => s.IsForward,
        ///       s => s.LightPunchPressed
        ///   )
        ///
        ///   // Shoryuken (forward, down, down-forward + punch)
        ///   buffer.Query(20).Sequence(
        ///       s => s.IsForward,
        ///       s => s.IsDown,
        ///       s => s.IsDownForward,
        ///       s => s.HeavyPunchPressed
        ///   )
        ///
        ///   // Super flash cancel — 2x QCF within 60 frames
        ///   buffer.Query(60).Sequence(
        ///       s => s.IsDown, s => s.IsDownForward, s => s.IsForward,
        ///       s => s.IsDown, s => s.IsDownForward, s => s.IsForward,
        ///       s => s.LightPunchPressed
        ///   )
        ///
        ///   // Double-tap forward dash (strict, no gaps)
        ///   buffer.Query(10).StrictSequence(
        ///       s => s.IsForward,
        ///       s => !s.IsForward,
        ///       s => s.IsForward
        ///   )
        ///
        ///   // Charge move — hold back 20 frames, then forward + punch
        ///   // (two-step Sequence; the charge check uses First() to confirm duration)
        ///   int chargeFrames = 0;
        ///   for (int age = buffer.Count - 1; age >= 0; age--)
        ///   {
        ///       if (buffer.PeekAt(age).IsBack) chargeFrames++;
        ///       else break;
        ///   }
        ///   bool charged = chargeFrames >= 20;
        ///   bool released = buffer.Query(5).Any(s => s.IsForward && s.PunchPressed);
        ///   if (charged && released) { /* fire sonic boom */ }
        /// </summary>
        public struct FighterSnapshot : IInputSnapshot
        {
            // IInputSnapshot
            public long Frame { get; set; }

            // ── Stick state ────────────────────────────────────────────────────────────
            public bool IsUp;
            public bool IsDown;
            public bool IsForward; // relative to character facing
            public bool IsBack;
            public bool IsDownForward;
            public bool IsDownBack;
            public bool IsUpForward;
            public bool IsUpBack;
            public bool IsNeutral;

            // ── Button presses (one-shot) ──────────────────────────────────────────────
            public bool LightPunchPressed;
            public bool MediumPunchPressed;
            public bool HeavyPunchPressed;
            public bool LightKickPressed;
            public bool MediumKickPressed;
            public bool HeavyKickPressed;

            // ── Button held ───────────────────────────────────────────────────────────
            public bool LightPunchHeld;
            public bool MediumPunchHeld;
            public bool HeavyPunchHeld;

            // ── Convenience shorthand ─────────────────────────────────────────────────
            public bool AnyPunchPressed => LightPunchPressed || MediumPunchPressed || HeavyPunchPressed;
            public bool AnyKickPressed => LightKickPressed || MediumKickPressed || HeavyKickPressed;
            public bool AnyButtonPressed => AnyPunchPressed || AnyKickPressed;
        }
    }

#endif
}