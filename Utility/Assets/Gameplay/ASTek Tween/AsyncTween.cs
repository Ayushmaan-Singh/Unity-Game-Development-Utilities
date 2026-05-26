using System;
using System.Threading;
using UnityEngine;

namespace Astek.Gameplay.TweenLibrary
{
    public static class AsyncTween
    {
        /// <summary>
        /// Custom DOVirtual.Float using Unity's new Awaitable system.
        /// </summary>
        public static async Awaitable Float(float from, float to, float duration, Action<float> onUpdate, CancellationToken? ctk = null)
        {
            float elapsed = 0f;

            while (elapsed < duration)
            {
                // Update time before the yield
                elapsed += Time.deltaTime;
                float t = Mathf.Clamp01(elapsed / duration);

                onUpdate?.Invoke(Mathf.Lerp(from, to, t));

                // Wait for the next frame (equivalent to yield return null)
                await (ctk != null ? Awaitable.NextFrameAsync(ctk.Value) : Awaitable.NextFrameAsync());
            }

            onUpdate?.Invoke(to);
        }
    }
}