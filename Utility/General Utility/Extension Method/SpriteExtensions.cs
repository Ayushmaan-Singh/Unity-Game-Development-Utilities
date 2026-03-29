using System;
using UnityEngine;

namespace Astek
{
    public static class SpriteExtensions
    {
        /// <summary>
        /// Gives the size of this sprite not considering object scale
        /// </summary>
        /// <param name="sprite"></param>
        /// <returns>Sprite to be checked</returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static Vector2 WorldUnscaledSize(this Sprite sprite) =>
            sprite != null ? sprite.rect.size / sprite.pixelsPerUnit : throw new ArgumentNullException(nameof(sprite));

        /// <summary>
        /// Get scaled size of sprite
        /// </summary>
        /// <param name="renderer"></param>
        /// <returns>Renderer containing the sprite to be checked</returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static Vector2 WorldScaledSize(this SpriteRenderer renderer) =>
            renderer == null ? throw new ArgumentNullException(nameof(renderer)) : renderer.sprite.WorldUnscaledSize() * renderer.transform.lossyScale;
    }
}