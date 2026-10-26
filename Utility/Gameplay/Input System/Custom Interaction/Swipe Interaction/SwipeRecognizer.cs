using System;
using UnityEngine;

namespace Astek.InputSystem
{
    /// <summary>
    /// Sliding-window swipe detector. Fires once per press when the pointer has
    /// travelled at least <see cref="MinDistance"/> within the last <see cref="Window"/> seconds.
    /// Zero allocation after construction.
    /// </summary>
    internal sealed class SwipeRecognizer
    {
        private const int Capacity = 64;
        private const double DuplicateTimeEpsilon = 0.001; // seconds

        private readonly Vector2[] _positions = new Vector2[Capacity];
        private readonly double[] _times = new double[Capacity];
        private int _start; // ring-buffer index of the oldest sample
        private int _count;

        /// <summary>Required travel inside the window, in the same units as the fed positions (pixels).</summary>
        public float MinDistance = 100f;

        /// <summary>Window length in seconds.</summary>
        public double Window = 0.1;

        /// <summary>True from the moment the swipe fires until the pointer is released.</summary>
        public bool Fired { get; private set; }

        /// <summary>Window displacement at the moment the swipe fired.</summary>
        public Vector2 Delta { get; private set; }

        public void Feed(bool pressed, Vector2 position, double time)
        {
            if (!pressed)
            {
                Reset();
                return;
            }

            if (Fired)
                return; // latched until release

            // The Input System may ask for the value several times per event.
            // Ignore a sample identical to the last one.
            if (_count > 0)
            {
                int last = Index(_count - 1);
                if (_positions[last] == position && time - _times[last] < DuplicateTimeEpsilon)
                    return;
            }

            Push(position, time);

            double cutoff = time - Math.Max(Window, 0.01);
            TrimTo(cutoff);

            Vector2 anchor = AnchorAt(cutoff);
            Vector2 delta = position - anchor;
            float min = Mathf.Max(MinDistance, 1f);

            if (delta.sqrMagnitude >= min * min)
            {
                Fired = true;
                Delta = delta;
            }
        }

        private int Index(int i) => (_start + i) % Capacity;

        private void Push(Vector2 position, double time)
        {
            if (_count == Capacity) // full: drop the oldest
            {
                _start = (_start + 1) % Capacity;
                _count--;
            }

            int slot = Index(_count);
            _positions[slot] = position;
            _times[slot]     = time;
            _count++;
        }

        // Drop the oldest sample while the second-oldest is already at or before the cutoff.
        // Afterwards the oldest sample is the last one at/before the cutoff (or the buffer
        // is simply younger than the window).
        private void TrimTo(double cutoff)
        {
            while (_count >= 2 && _times[Index(1)] <= cutoff)
            {
                _start = (_start + 1) % Capacity;
                _count--;
            }
        }

        // Position of the pointer at exactly "cutoff", interpolated between the two samples
        // that straddle it. If the buffer is younger than the window, use the oldest sample.
        private Vector2 AnchorAt(double cutoff)
        {
            int i0 = Index(0);
            if (_count < 2 || _times[i0] >= cutoff)
                return _positions[i0];

            int i1 = Index(1);
            // Here t0 < cutoff < t1 (guaranteed by TrimTo), so t1 - t0 > 0.
            float f = (float)((cutoff - _times[i0]) / (_times[i1] - _times[i0]));
            return Vector2.Lerp(_positions[i0], _positions[i1], f);
        }
        
        public void Reset()
        {
            _start = 0;
            _count = 0;
            Fired  = false;
            Delta  = Vector2.zero;
        }
    }
}