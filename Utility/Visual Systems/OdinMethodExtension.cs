using System;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;

namespace AstekUtility.VisualFeedback
{
	public static class OdinMethodExtension
	{
		#if UNITY_EDITOR
		/// <summary>
		/// Always mark the Attribute to be run in editor only
		/// </summary>
		/// <param name="objRenderer"></param>
		/// <returns></returns>
		/// <exception cref="ArgumentOutOfRangeException"></exception>
		private static ValueDropdownList<int> MaterialProperties(this Renderer objRenderer)
		{
			if (!objRenderer)
				return new ValueDropdownList<int>();
			
			Shader shader = objRenderer.sharedMaterial.shader;
			int propertyCount = shader.GetPropertyCount();
			ValueDropdownList<int> valueDropDownList = new ValueDropdownList<int>();

			if (objRenderer == null || objRenderer.sharedMaterial == null)
				return valueDropDownList;

			for (int i = 0; i < propertyCount; i++)
			{
				ShaderUtil.ShaderPropertyType type = ShaderUtil.GetPropertyType(shader, i);
				string propertyName = ShaderUtil.GetPropertyName(shader, i);
				valueDropDownList.Add($"{PropertyTypeToString(type)}/{propertyName}", Shader.PropertyToID(propertyName));
			}

			return valueDropDownList;

			string PropertyTypeToString(ShaderUtil.ShaderPropertyType type)
			{
				return type switch
				{
					ShaderUtil.ShaderPropertyType.Color => "Color",
					ShaderUtil.ShaderPropertyType.Vector => "Vector",
					ShaderUtil.ShaderPropertyType.Float => "Float",
					ShaderUtil.ShaderPropertyType.Range => "Range",
					ShaderUtil.ShaderPropertyType.TexEnv => "TexEnv",
					ShaderUtil.ShaderPropertyType.Int => "Int",
					_ => throw new ArgumentOutOfRangeException()
				};
			}
		}
		#endif
	}
}