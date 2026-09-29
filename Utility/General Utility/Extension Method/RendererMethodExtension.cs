using System;
using System.Collections.Generic;
using UnityEngine;

namespace Astek
{
    public static class RendererMethodExtension
    {
        #region Modify Material Properties

        private static MaterialPropertyBlock _cachedBlock = new MaterialPropertyBlock();

        public static void ModifyMaterialProperty_Float(this Renderer renderer, int propertyID, float val)
        {
            if (!renderer) throw new ArgumentNullException(nameof(renderer));
            renderer.GetPropertyBlock(_cachedBlock);

            if (_cachedBlock == null) throw new NullReferenceException($"{renderer} doesn't have a material");
            if (!_cachedBlock.HasFloat(propertyID)) throw new Exception($"Material:{renderer.material} doesn't have a property by id {propertyID}");
            _cachedBlock.SetFloat(propertyID, val);

            renderer.SetPropertyBlock(_cachedBlock);
            _cachedBlock.Clear();
        }
        public static void ModifyMaterialProperty_FloatArray(this Renderer renderer, int propertyID, List<float> val)
        {
            if (!renderer) throw new ArgumentNullException(nameof(renderer));
            renderer.GetPropertyBlock(_cachedBlock);

            if (_cachedBlock == null) throw new NullReferenceException($"{renderer} doesn't have a material");
            if (_cachedBlock.GetFloatArray(propertyID) == null) throw new Exception($"Material:{renderer.material} doesn't have a property by id {propertyID}");
            _cachedBlock.SetFloatArray(propertyID, val);

            renderer.SetPropertyBlock(_cachedBlock);
            _cachedBlock.Clear();
        }

        public static void ModifyMaterialProperty_Int(this Renderer renderer, int propertyID, int val)
        {
            if (!renderer) throw new ArgumentNullException(nameof(renderer));
            renderer.GetPropertyBlock(_cachedBlock);

            if (_cachedBlock == null) throw new NullReferenceException($"{renderer} doesn't have a material");
            if (!_cachedBlock.HasInt(propertyID)) throw new Exception($"Material:{renderer.material} doesn't have a property by id {propertyID}");
            _cachedBlock.SetInteger(propertyID, val);

            renderer.SetPropertyBlock(_cachedBlock);
            _cachedBlock.Clear();
        }

        public static void ModifyMaterialProperty_Vector(this Renderer renderer, int propertyID, Vector4 val)
        {
            if (!renderer) throw new ArgumentNullException(nameof(renderer));
            renderer.GetPropertyBlock(_cachedBlock);

            if (_cachedBlock == null) throw new NullReferenceException($"{renderer} doesn't have a material");
            if (!_cachedBlock.HasVector(propertyID)) throw new Exception($"Material:{renderer.material} doesn't have a property by id {propertyID}");
            _cachedBlock.SetVector(propertyID, val);

            renderer.SetPropertyBlock(_cachedBlock);
            _cachedBlock.Clear();
        }
        public static void ModifyMaterialProperty_VectorArray(this Renderer renderer, int propertyID, List<Vector4> val)
        {
            if (!renderer) throw new ArgumentNullException(nameof(renderer));
            renderer.GetPropertyBlock(_cachedBlock);

            if (_cachedBlock == null) throw new NullReferenceException($"{renderer} doesn't have a material");
            if (_cachedBlock.GetVectorArray(propertyID) == null)
                throw new Exception($"Material:{renderer.material} doesn't have a property by id {propertyID}");
            _cachedBlock.SetVectorArray(propertyID, val);

            renderer.SetPropertyBlock(_cachedBlock);
            _cachedBlock.Clear();
        }

        public static void ModifyMaterialProperty_Matrix(this Renderer renderer, int propertyID, Matrix4x4 val)
        {
            if (!renderer) throw new ArgumentNullException(nameof(renderer));
            renderer.GetPropertyBlock(_cachedBlock);

            if (_cachedBlock == null) throw new NullReferenceException($"{renderer} doesn't have a material");
            if (!_cachedBlock.HasMatrix(propertyID)) throw new Exception($"Material:{renderer.material} doesn't have a property by id {propertyID}");
            _cachedBlock.SetMatrix(propertyID, val);

            renderer.SetPropertyBlock(_cachedBlock);
            _cachedBlock.Clear();
        }
        public static void ModifyMaterialProperty_MatrixArray(this Renderer renderer, int propertyID, List<Matrix4x4> val)
        {
            if (!renderer) throw new ArgumentNullException(nameof(renderer));
            renderer.GetPropertyBlock(_cachedBlock);

            if (_cachedBlock == null) throw new NullReferenceException($"{renderer} doesn't have a material");
            if (_cachedBlock.GetMatrixArray(propertyID) == null)
                throw new Exception($"Material:{renderer.material} doesn't have a property by id {propertyID}");
            _cachedBlock.SetMatrixArray(propertyID, val);

            renderer.SetPropertyBlock(_cachedBlock);
            _cachedBlock.Clear();
        }

        public static void ModifyMaterialProperty_Texture(this Renderer renderer, int propertyID, Texture val)
        {
            if (!renderer) throw new ArgumentNullException(nameof(renderer));
            renderer.GetPropertyBlock(_cachedBlock);

            if (_cachedBlock == null) throw new NullReferenceException($"{renderer} doesn't have a material");
            if (!_cachedBlock.HasTexture(propertyID)) throw new Exception($"Material:{renderer.material} doesn't have a property by id {propertyID}");
            _cachedBlock.SetTexture(propertyID, val);

            renderer.SetPropertyBlock(_cachedBlock);
            _cachedBlock.Clear();
        }

        public static void ModifyMaterialProperty_Buffer(this Renderer renderer, int propertyID, ComputeBuffer val)
        {
            if (!renderer) throw new ArgumentNullException(nameof(renderer));
            renderer.GetPropertyBlock(_cachedBlock);

            if (_cachedBlock == null) throw new NullReferenceException($"{renderer} doesn't have a material");
            if (!_cachedBlock.HasBuffer(propertyID)) throw new Exception($"Material:{renderer.material} doesn't have a property by id {propertyID}");
            _cachedBlock.SetBuffer(propertyID, val);

            renderer.SetPropertyBlock(_cachedBlock);
            _cachedBlock.Clear();
        }

        public static void ModifyMaterialProperty_Color(this Renderer renderer, int propertyID, Color val)
        {
            if (!renderer) throw new ArgumentNullException(nameof(renderer));
            renderer.GetPropertyBlock(_cachedBlock);

            if (_cachedBlock == null) throw new NullReferenceException($"{renderer} doesn't have a material");
            if (!_cachedBlock.HasColor(propertyID)) throw new Exception($"Material:{renderer.material} doesn't have a property by id {propertyID}");
            _cachedBlock.SetColor(propertyID, val);

            renderer.SetPropertyBlock(_cachedBlock);
            _cachedBlock.Clear();
        }

        #endregion

        #region Line Renderer

        public static void Clear(this LineRenderer renderer)
        {
            if (renderer == null) throw new ArgumentNullException(nameof(renderer));
            renderer.positionCount = 0;
        }

        #endregion
    }
}