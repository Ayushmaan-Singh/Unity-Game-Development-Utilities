using System.Collections.Generic;
using UnityEngine;
namespace AstekUtility
{
	public static class RendererMethodExtension
	{
		#region Modify Material Properties

		public static void ModifyMaterialProperty_Float(this Renderer renderer, int propertyID, float val)
		{
			MaterialPropertyBlock cachePropertyBlock = new MaterialPropertyBlock();
			renderer.GetPropertyBlock(cachePropertyBlock);

			cachePropertyBlock.SetFloat(propertyID, val);

			renderer.SetPropertyBlock(cachePropertyBlock);
			cachePropertyBlock.Clear();
		}
		public static void ModifyMaterialProperty_FloatArray(this Renderer renderer, int propertyID, List<float> val)
		{
			MaterialPropertyBlock cachePropertyBlock = new MaterialPropertyBlock();
			renderer.GetPropertyBlock(cachePropertyBlock);

			cachePropertyBlock.SetFloatArray(propertyID, val);

			renderer.SetPropertyBlock(cachePropertyBlock);
			cachePropertyBlock.Clear();
		}

		public static void ModifyMaterialProperty_Int(this Renderer renderer, int propertyID, int val)
		{
			MaterialPropertyBlock cachePropertyBlock = new MaterialPropertyBlock();
			renderer.GetPropertyBlock(cachePropertyBlock);

			cachePropertyBlock.SetInteger(propertyID, val);

			renderer.SetPropertyBlock(cachePropertyBlock);
			cachePropertyBlock.Clear();
		}

		public static void ModifyMaterialProperty_Vector(this Renderer renderer, int propertyID, Vector4 val)
		{
			MaterialPropertyBlock cachePropertyBlock = new MaterialPropertyBlock();
			renderer.GetPropertyBlock(cachePropertyBlock);

			cachePropertyBlock.SetVector(propertyID, val);

			renderer.SetPropertyBlock(cachePropertyBlock);
			cachePropertyBlock.Clear();
		}
		public static void ModifyMaterialProperty_VectorArray(this Renderer renderer, int propertyID, List<Vector4> val)
		{
			MaterialPropertyBlock cachePropertyBlock = new MaterialPropertyBlock();
			renderer.GetPropertyBlock(cachePropertyBlock);

			cachePropertyBlock.SetVectorArray(propertyID, val);

			renderer.SetPropertyBlock(cachePropertyBlock);
			cachePropertyBlock.Clear();
		}

		public static void ModifyMaterialProperty_Matrix(this Renderer renderer, int propertyID, Matrix4x4 val)
		{
			MaterialPropertyBlock cachePropertyBlock = new MaterialPropertyBlock();
			renderer.GetPropertyBlock(cachePropertyBlock);

			cachePropertyBlock.SetMatrix(propertyID, val);

			renderer.SetPropertyBlock(cachePropertyBlock);
			cachePropertyBlock.Clear();
		}
		public static void ModifyMaterialProperty_MatrixArray(this Renderer renderer, int propertyID, List<Matrix4x4> val)
		{
			MaterialPropertyBlock cachePropertyBlock = new MaterialPropertyBlock();
			renderer.GetPropertyBlock(cachePropertyBlock);

			cachePropertyBlock.SetMatrixArray(propertyID, val);

			renderer.SetPropertyBlock(cachePropertyBlock);
			cachePropertyBlock.Clear();
		}

		public static void ModifyMaterialProperty_Texture(this Renderer renderer, int propertyID, Texture val)
		{
			MaterialPropertyBlock cachePropertyBlock = new MaterialPropertyBlock();
			renderer.GetPropertyBlock(cachePropertyBlock);

			cachePropertyBlock.SetTexture(propertyID, val);

			renderer.SetPropertyBlock(cachePropertyBlock);
			cachePropertyBlock.Clear();
		}

		public static void ModifyMaterialProperty_Buffer(this Renderer renderer, int propertyID, ComputeBuffer val)
		{
			MaterialPropertyBlock cachePropertyBlock = new MaterialPropertyBlock();
			renderer.GetPropertyBlock(cachePropertyBlock);

			cachePropertyBlock.SetBuffer(propertyID, val);

			renderer.SetPropertyBlock(cachePropertyBlock);
			cachePropertyBlock.Clear();
		}

		public static void ModifyMaterialProperty_Color(this Renderer renderer, int propertyID, Color val)
		{
			MaterialPropertyBlock cachePropertyBlock = new MaterialPropertyBlock();
			renderer.GetPropertyBlock(cachePropertyBlock);

			cachePropertyBlock.SetColor(propertyID, val);

			renderer.SetPropertyBlock(cachePropertyBlock);
			cachePropertyBlock.Clear();
		}

		#endregion

		#region Line Renderer

		public static void Clear(this LineRenderer renderer) => renderer.positionCount = 0;

		#endregion
	}
}