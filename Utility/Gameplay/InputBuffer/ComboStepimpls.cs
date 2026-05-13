using System;

namespace Astek.InputBuffer
{
// ── Built-in step implementations ─────────────────────────────────────────────
// Each class is self-contained. Adding a new pattern = new file, zero edits here.
// All steps are structs to avoid heap allocation per-combo-check.
// ─────────────────────────────────────────────────────────────────────────────

// ── PressStep ─────────────────────────────────────────────────────────────────

/// <summary>
/// Satisfied by any single frame where predicate is true.
/// Scans oldest → newest; consumes the first matching frame.
/// Gaps before the frame are silently skipped.
///
///   ComboStep&lt;T&gt;.Press(s => s.AttackPressed)
/// </summary>
public readonly struct PressStep<T> : IComboStep<T> where T : IInputSnapshot
{
    private readonly Func<T, bool> _predicate;
    public PressStep(Func<T, bool> predicate) => _predicate = predicate;

    public bool TryMatch(InputBuffer<T> buffer, int cursor, out int newCursor)
    {
        for (int age = cursor; age >= 0; age--)
        {
            if (_predicate(buffer.PeekAt(age)))
            {
                newCursor = age - 1;   // next step searches frames newer than this
                return true;
            }
        }
        newCursor = default;
        return false;
    }
}

// ── HoldStep ─────────────────────────────────────────────────────────────────

/// <summary>
/// Satisfied when predicate is true for at least <see cref="MinFrames"/> consecutive frames.
/// Finds the first qualifying run, oldest → newest. Gaps before the run are allowed.
///
///   ComboStep&lt;T&gt;.Hold(s => s.IsDown, 20)
/// </summary>
public readonly struct HoldStep<T> : IComboStep<T> where T : IInputSnapshot
{
    private readonly Func<T, bool> _predicate;
    public readonly int MinFrames;

    public HoldStep(Func<T, bool> predicate, int minFrames)
    {
        _predicate = predicate;
        MinFrames  = Math.Max(1, minFrames);
    }

    public bool TryMatch(InputBuffer<T> buffer, int cursor, out int newCursor)
    {
        int age = cursor;
        while (age >= 0)
        {
            if (!_predicate(buffer.PeekAt(age))) { age--; continue; }

            // Start of a potential run — count consecutive matches forward in time
            int runStart = age;
            int run      = 0;
            for (int a = runStart; a >= 0 && _predicate(buffer.PeekAt(a)); a--)
                run++;

            if (run >= MinFrames)
            {
                // Consume exactly MinFrames from the start of the run
                newCursor = Math.Max(runStart - MinFrames, -1);
                return true;
            }

            // Run too short — skip past it and keep searching
            age = runStart - run - 1;
        }
        newCursor = default;
        return false;
    }
}

// ── HoldBetweenStep ───────────────────────────────────────────────────────────

/// <summary>
/// Satisfied when a run of consecutive matching frames has a length
/// between <see cref="MinFrames"/> and <see cref="MaxFrames"/> (inclusive).
///
/// The run must be exact — the frame immediately before the run (one frame older)
/// must NOT match (or be at the buffer boundary). This ensures "held for 10–20 frames"
/// doesn't match a player who held for 30 frames straight.
///
///   ComboStep&lt;T&gt;.HoldBetween(s => s.IsBack, 10, 25)   // between 10 and 25 frames
/// </summary>
public readonly struct HoldBetweenStep<T> : IComboStep<T> where T : IInputSnapshot
{
    private readonly Func<T, bool> _predicate;
    public readonly int MinFrames;
    public readonly int MaxFrames;

    public HoldBetweenStep(Func<T, bool> predicate, int minFrames, int maxFrames)
    {
        _predicate = predicate;
        MinFrames  = Math.Max(1, minFrames);
        MaxFrames  = Math.Max(MinFrames, maxFrames);
    }

    public bool TryMatch(InputBuffer<T> buffer, int cursor, out int newCursor)
    {
        int age = cursor;
        while (age >= 0)
        {
            if (!_predicate(buffer.PeekAt(age))) { age--; continue; }

            // Potential run start — but only if the frame just before it did NOT match
            // (or we're at the buffer boundary). This enforces the "exact hold" semantic.
            bool cleanStart = (age == cursor)                     // at search boundary
                           || (age + 1 <= cursor                  // frame before exists
                               && !_predicate(buffer.PeekAt(age + 1))); // and doesn't match

            if (!cleanStart) { age--; continue; }

            // Measure the full run length
            int runStart = age;
            int run      = 0;
            for (int a = runStart; a >= 0 && run <= MaxFrames && _predicate(buffer.PeekAt(a)); a--)
                run++;

            if (run >= MinFrames && run <= MaxFrames)
            {
                newCursor = Math.Max(runStart - run, -1);   // just past the end of this run
                return true;
            }

            // Run is too short or too long — skip it entirely
            age = runStart - run - 1;
        }
        newCursor = default;
        return false;
    }
}

// ── HoldAfterStep ─────────────────────────────────────────────────────────────

/// <summary>
/// Satisfied when predicate is true on EVERY frame from the cursor to age 0.
/// Verifies the player is STILL holding at the moment of the query.
/// Should always be the last step in a sequence.
///
///   ComboStep&lt;T&gt;.HoldAfter(s => s.HeavyHeld, minFrames: 1)
/// </summary>
public readonly struct HoldAfterStep<T> : IComboStep<T> where T : IInputSnapshot
{
    private readonly Func<T, bool> _predicate;
    private readonly int           _minFrames;

    public HoldAfterStep(Func<T, bool> predicate, int minFrames = 1)
    {
        _predicate = predicate;
        _minFrames = Math.Max(1, minFrames);
    }

    public bool TryMatch(InputBuffer<T> buffer, int cursor, out int newCursor)
    {
        int remaining = cursor + 1;
        if (remaining < _minFrames) { newCursor = default; return false; }

        for (int age = cursor; age >= 0; age--)
        {
            if (!_predicate(buffer.PeekAt(age))) { newCursor = default; return false; }
        }

        newCursor = -1;   // consumed all remaining frames
        return true;
    }
}

// ── ReleaseStep ───────────────────────────────────────────────────────────────

/// <summary>
/// Satisfied when a button transitions from true → false within the window.
/// Finds the first frame where predicate is true followed immediately by a frame
/// where it is false (or the sequence reaches age 0).
/// Useful for "tap and release" timing checks.
///
///   ComboStep&lt;T&gt;.Release(s => s.HeavyPunchHeld)
/// </summary>
public readonly struct ReleaseStep<T> : IComboStep<T> where T : IInputSnapshot
{
    private readonly Func<T, bool> _predicate;
    public ReleaseStep(Func<T, bool> predicate) => _predicate = predicate;

    public bool TryMatch(InputBuffer<T> buffer, int cursor, out int newCursor)
    {
        // Find a frame where predicate is true, then the very next frame it's false
        for (int age = cursor; age >= 1; age--)
        {
            if (_predicate(buffer.PeekAt(age)) && !_predicate(buffer.PeekAt(age - 1)))
            {
                newCursor = Math.Max(age - 2, -1);   // past the release frame, clamped
                return true;
            }
        }
        newCursor = default;
        return false;
    }
}

// ── AnyOfStep ─────────────────────────────────────────────────────────────────

/// <summary>
/// Satisfied when ANY of the provided sub-steps matches at the current cursor.
/// Acts as a logical OR over multiple step types.
/// The first sub-step that matches wins; its newCursor is used.
///
///   ComboStep&lt;T&gt;.AnyOf(
///       ComboStep&lt;T&gt;.Press(s => s.LightPunchPressed),
///       ComboStep&lt;T&gt;.Press(s => s.MediumPunchPressed)
///   )
/// </summary>
public readonly struct AnyOfStep<T> : IComboStep<T> where T : IInputSnapshot
{
    private readonly IComboStep<T>[] _steps;
    public AnyOfStep(params IComboStep<T>[] steps) => _steps = steps;

    public bool TryMatch(InputBuffer<T> buffer, int cursor, out int newCursor)
    {
        foreach (var step in _steps)
        {
            if (step.TryMatch(buffer, cursor, out newCursor)) return true;
        }
        newCursor = default;
        return false;
    }
}

// ── NegateStep ────────────────────────────────────────────────────────────────

/// <summary>
/// Satisfied when the inner step FAILS.
/// Advances cursor by exactly one frame when the inner step doesn't match there.
/// Useful for asserting a button is NOT pressed at this point in a sequence.
///
///   ComboStep&lt;T&gt;.Not(ComboStep&lt;T&gt;.Press(s => s.IsForward))
/// </summary>
public readonly struct NegateStep<T> : IComboStep<T> where T : IInputSnapshot
{
    private readonly IComboStep<T> _inner;
    public NegateStep(IComboStep<T> inner) => _inner = inner;

    public bool TryMatch(InputBuffer<T> buffer, int cursor, out int newCursor)
    {
        // Succeeds when inner fails at the current cursor frame
        if (!_inner.TryMatch(buffer, cursor, out _))
        {
            newCursor = cursor - 1;
            return true;
        }
        newCursor = default;
        return false;
    }
}
}