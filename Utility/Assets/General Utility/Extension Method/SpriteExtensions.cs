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
        
        /// <summary>
        /// Offset a sprite holding gameobject by certain pixel in comparison to sprite
        /// </summary>
        /// <param name="renderer"></param>
        /// <param name="pixelOffset"></param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="NullReferenceException"></exception>
        public static void OffsetByPixels(this SpriteRenderer renderer, Vector2 pixelOffset)
        {
            if (renderer == null) 
                throw new ArgumentNullException(nameof(renderer));
            if(renderer.sprite == null)
                throw new NullReferenceException(nameof(renderer.sprite));

            // 1. Convert pixels to base world units
            float ppu = renderer.sprite.pixelsPerUnit;
            Vector2 unitOffset = pixelOffset / ppu;

            // 2. Account for the GameObject's scale
            Vector3 worldOffset = Vector3.Scale(unitOffset, renderer.transform.lossyScale);

            // 3. Apply to position
            renderer.transform.position += worldOffset;
        }
    }
}