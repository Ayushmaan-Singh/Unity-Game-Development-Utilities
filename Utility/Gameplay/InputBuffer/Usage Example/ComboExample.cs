using System;

namespace Astek.InputBuffer
{
#if UNITY_EDITOR

    namespace EditorOnly
    {
         // ── ComboExamples.cs ──────────────────────────────────────────────────────────
        // Annotated usage of every built-in step type.
        // This file is documentation — not a MonoBehaviour.
        // ─────────────────────────────────────────────────────────────────────────────

        using UnityEngine;

        public static class ComboExamples
        {
            // Short aliases
            private static IComboStep<PlatformerSnapshot> P(
                Func<PlatformerSnapshot, bool> pred) => ComboStep<PlatformerSnapshot>.Press(pred);

            private static IComboStep<FighterSnapshot> F(
                Func<FighterSnapshot, bool> pred) => ComboStep<FighterSnapshot>.Press(pred);

            // ─────────────────────────────────────────────────────────────────────────
            //  PLATFORMER
            // ─────────────────────────────────────────────────────────────────────────

            /// <summary>
            /// THE EXACT REQUESTED CASE:
            /// Hold down (20f charge) → press attack → hold heavy right now.
            /// </summary>
            public static bool ChargeAttackHoldHeavy(InputBuffer<PlatformerSnapshot> buf)
                => buf.Query(60).ComboSequence(
                    ComboStep<PlatformerSnapshot>.Hold(s => s.InputY < 0f, 20),
                    ComboStep<PlatformerSnapshot>.Press(s => s.AttackPressed),
                    ComboStep<PlatformerSnapshot>.HoldAfter(s => s.HeavyHeld, 1)
                );

            /// <summary>
            /// HoldBetween: short charge (5–15 frames) then attack.
            /// "Tap charge" — too short = no effect, too long = different move.
            /// </summary>
            public static bool TapChargeAttack(InputBuffer<PlatformerSnapshot> buf)
                => buf.Query(30).ComboSequence(
                    ComboStep<PlatformerSnapshot>.HoldBetween(s => s.InputY < 0f, 5, 15),
                    ComboStep<PlatformerSnapshot>.Press(s => s.AttackPressed)
                );

            /// <summary>
            /// Standalone HeldBetween — no sequence, just a timing window check.
            /// </summary>
            public static bool ShortCrouch(InputBuffer<PlatformerSnapshot> buf)
                => buf.Query(30).HeldBetween(s => s.InputY < 0f, 3, 12);

            /// <summary>
            /// Double-tap dash — strict, release required between taps.
            /// StrictSequence is still available for frame-exact patterns.
            /// </summary>
            public static bool DoubleTapDash(InputBuffer<PlatformerSnapshot> buf)
                => buf.Query(10).StrictSequence(
                    s => s.DashPressed,
                    s => !s.DashPressed,
                    s => s.DashPressed
                );

            /// <summary>
            /// Coyote time — Any query, no sequence needed.
            /// </summary>
            public static bool CoyoteTime(InputBuffer<PlatformerSnapshot> buf, long lastConsumed)
                => buf.Query(6).Any(s => s.OnGround)
                    && buf.Query(4).Any(s => s.JumpPressed && s.Frame > lastConsumed);

            // ─────────────────────────────────────────────────────────────────────────
            //  FIGHTER
            // ─────────────────────────────────────────────────────────────────────────

            /// <summary>
            /// Hadouken — QCF + any punch, gaps allowed, 30 frame window.
            /// Classic Sequence (no hold requirement).
            /// </summary>
            public static bool Hadouken(InputBuffer<FighterSnapshot> buf)
                => buf.Query(30).Sequence(
                    s => s.IsDown,
                    s => s.IsDownForward,
                    s => s.IsForward,
                    s => s.AnyPunchPressed
                );

            /// <summary>
            /// Sonic Boom — hold back 20f, then forward + punch.
            /// Hold + two presses.
            /// </summary>
            public static bool SonicBoom(InputBuffer<FighterSnapshot> buf)
                => buf.Query(50).ComboSequence(
                    ComboStep<FighterSnapshot>.Hold(s => s.IsBack, 20),
                    ComboStep<FighterSnapshot>.Press(s => s.IsForward),
                    ComboStep<FighterSnapshot>.Press(s => s.AnyPunchPressed)
                );

            /// <summary>
            /// Parry window — held back between 2 and 6 frames exactly, then forward.
            /// Too short = miss, too long = block instead of parry.
            /// </summary>
            public static bool Parry(InputBuffer<FighterSnapshot> buf)
                => buf.Query(20).ComboSequence(
                    ComboStep<FighterSnapshot>.HoldBetween(s => s.IsBack, 2, 6),
                    ComboStep<FighterSnapshot>.Press(s => s.IsForward)
                );

            /// <summary>
            /// Held punch release — charge heavy punch 20–40 frames, then release.
            /// HoldBetween + Release: "charged shot" that has a max charge time.
            /// </summary>
            public static bool ChargedPunch(InputBuffer<FighterSnapshot> buf)
                => buf.Query(60).ComboSequence(
                    ComboStep<FighterSnapshot>.HoldBetween(s => s.HeavyPunchHeld, 20, 40),
                    ComboStep<FighterSnapshot>.Release(s => s.HeavyPunchHeld)
                );

            /// <summary>
            /// AnyOf combinator — accept light OR heavy punch at the end of QCF.
            /// </summary>
            public static bool HadoukenAnyStrength(InputBuffer<FighterSnapshot> buf)
                => buf.Query(30).ComboSequence(
                    ComboStep<FighterSnapshot>.Press(s => s.IsDown),
                    ComboStep<FighterSnapshot>.Press(s => s.IsDownForward),
                    ComboStep<FighterSnapshot>.Press(s => s.IsForward),
                    ComboStep<FighterSnapshot>.AnyOf(
                        ComboStep<FighterSnapshot>.Press(s => s.LightPunchPressed),
                        ComboStep<FighterSnapshot>.Press(s => s.HeavyPunchPressed)
                    )
                );

            /// <summary>
            /// Not combinator — ensure player is NOT holding back when punch connects.
            /// Useful for filtering accidental inputs.
            /// </summary>
            public static bool ForwardPunchNotBlocking(InputBuffer<FighterSnapshot> buf)
                => buf.Query(20).ComboSequence(
                    ComboStep<FighterSnapshot>.Not(ComboStep<FighterSnapshot>.Press(s => s.IsBack)),
                    ComboStep<FighterSnapshot>.Press(s => s.AnyPunchPressed)
                );

            // ─────────────────────────────────────────────────────────────────────────
            //  ADDING YOUR OWN PATTERN (example template)
            // ─────────────────────────────────────────────────────────────────────────

            // To add e.g. a Timeout step (gap of N frames where nothing is pressed):
            //
            //   1. Create TimeoutStep.cs:
            //
            //      public readonly struct TimeoutStep<T> : IComboStep<T> where T : IInputSnapshot
            //      {
            //          private readonly Func<T, bool> _mustBeFalse;
            //          private readonly int           _exactFrames;
            //          public TimeoutStep(Func<T, bool> mustBeFalse, int exactFrames) { ... }
            //          public bool TryMatch(InputBuffer<T> buffer, int cursor, out int newCursor) { ... }
            //      }
            //
            //   2. Add to ComboStep<T>:
            //
            //      public static IComboStep<T> Timeout(Func<T, bool> mustBeFalse, int frames)
            //          => new TimeoutStep<T>(mustBeFalse, frames);
            //
            //   3. Use immediately — zero changes to InputBufferQuery or any existing file.
        }
    }
#endif
}