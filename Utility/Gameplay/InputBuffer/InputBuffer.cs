using System;

namespace Astek.InputBuffer
{
    public class InputBuffer<T> where T : IInputSnapshot
    {
        private readonly T[] _buffer;
        private int _head = -1;
        private int _count = 0;
        private long _frameCounter = 0;

        /// <summary>Maximum frames the buffer can store.</summary>
        public int Capacity => _buffer.Length;

        /// <summary>Frames actually stored (ramps up to Capacity, then stays there).</summary>
        public int Count => _count;

        /// <summary>Frame index assigned to the last Record() call. -1 if nothing recorded yet.</summary>
        public long CurrentFrame => _frameCounter - 1;

        public InputBuffer(int capacity = 64)
        {
            if (capacity <= 0)
                throw new ArgumentOutOfRangeException(nameof(capacity));
            _buffer = new T[capacity];
        }

        // ── Write ──────────────────────────────────────────────────────────────────

        /// <summary>
        /// Record one snapshot for the current frame.
        /// Stamps snapshot.Frame, increments the frame counter, stores the entry.
        ///
        /// Call once per FixedUpdate, AFTER:
        ///   1. Input callbacks have fired (they always do before FixedUpdate in Unity)
        ///   2. Contact detection (OnGround, WallDir) has run
        /// so the snapshot reflects the true physics + input state for this tick.
        /// </summary>
        public void Record(T snapshot)
        {
            snapshot.Frame = _frameCounter++;
            _head          = (_head + 1) % _buffer.Length;
            _buffer[_head] = snapshot;
            if (_count < _buffer.Length)
                _count++;
        }

        // ── Read ───────────────────────────────────────────────────────────────────

        /// <summary>
        /// Direct snapshot access by age.
        ///   age 0 = most recent frame
        ///   age 1 = one frame ago
        /// Throws ArgumentOutOfRangeException if age >= Count.
        /// </summary>
        public T PeekAt(int age)
        {
            if ((uint)age >= (uint)_count)
                throw new ArgumentOutOfRangeException(nameof(age), $"age {age} out of range — only {_count} frames stored.");

            int idx = ((_head - age) % _buffer.Length + _buffer.Length) % _buffer.Length;
            return _buffer[idx];
        }

        /// <summary>
        /// Returns a query scoped to the last <paramref name="windowFrames"/> frames.
        /// Automatically clamped to Count so you never need to guard the call site.
        /// </summary>
        public InputBufferQuery<T> Query(int windowFrames)
        {
            return new InputBufferQuery<T>(this, Math.Min(windowFrames, _count));
        }
    }
}