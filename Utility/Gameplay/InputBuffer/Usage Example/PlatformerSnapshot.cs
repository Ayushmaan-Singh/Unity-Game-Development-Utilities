namespace Astek.InputBuffer
{
#if UNITY_EDITOR

    namespace EditorOnly
    {
        /// <summary>
        /// One frame of platformer input + physics state.
        /// Recorded once per FixedUpdate in MovementController.
        ///
        /// Struct so Record() stores by value — no heap allocation per frame.
        ///
        /// Physics fields (OnGround, WallDir) are included so coyote time
        /// and wall-slide history are queryable from the same buffer as input.
        /// </summary>
        public struct PlatformerSnapshot : IInputSnapshot
        {
            // ── IInputSnapshot ─────────────────────────────────────────────────────────
            public long Frame { get; set; } // stamped by InputBuffer.Record()

            // ── Continuous input ───────────────────────────────────────────────────────
            public float InputX; // -1 / 0 / +1
            public float InputY;

            // ── One-shot presses ───────────────────────────────────────────────────────
            // Set to true for the single frame the button transitioned pressed.
            // The buffer preserves these — they don't need separate consume flags
            // because Frame-based consumption (s.Frame > lastConsumedFrame) handles it.
            public bool JumpPressed;
            public bool DashPressed;

            // ── Held buttons ──────────────────────────────────────────────────────────
            public bool JumpHeld;
            public bool GrabHeld;
            public bool HeavyHeld; // heavy attack button held (for HoldAfter combos)

            // ── Attack presses (one-shot) ──────────────────────────────────────────────
            public bool AttackPressed; // light / normal attack
            public bool HeavyPressed; // heavy attack tap

            // ── Physics state at time of snapshot ─────────────────────────────────────
            // Recorded AFTER contact detection so these reflect the true physics state.
            public bool OnGround;
            public bool OnWall;
            public int WallDir; // -1 left | 0 none | +1 right
        }
    }
#endif
}