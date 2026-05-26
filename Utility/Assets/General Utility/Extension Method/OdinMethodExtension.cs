using System;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
using AnimatorController = UnityEditor.Animations.AnimatorController;
using AnimatorControllerLayer = UnityEditor.Animations.AnimatorControllerLayer;
using AnimatorControllerParameter = UnityEngine.AnimatorControllerParameter;
using AnimatorControllerParameterType = UnityEngine.AnimatorControllerParameterType;

namespace Astek
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
		public static ValueDropdownList<int> MaterialProperties(this Renderer objRenderer)
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

		public static ValueDropdownList<AnimatorParameter> GetAnimatorParameters(this Animator animatorController)
		{
			ValueDropdownList<AnimatorParameter> dropdownList = new ValueDropdownList<AnimatorParameter>();
			if (animatorController != null)
			{
				AnimatorControllerParameter[] parameters=animatorController.parameters;
				parameters.ForEach(parameter =>
				{
					dropdownList.Add($"{parameter.type.ToString()}/{parameter.name}",new AnimatorParameter()
					{
						Type = parameter.type,
						ParameterHash = parameter.nameHash
					});
				});
			}
			return dropdownList;
		}

		public static ValueDropdownList<AnimationData> GetAnimatorStates(this Animator animatorController)
		{
			ValueDropdownList<AnimationData> dropdownList = new ValueDropdownList<AnimationData>();
			if (animatorController != null && animatorController.runtimeAnimatorController is AnimatorController controller)
			{
				for (int i = 0; i < controller.layers.Length; i++)
				{
					AnimatorControllerLayer layer = controller.layers[i];
					UnityEditor.Animations.AnimatorStateMachine stateMachine = layer.stateMachine;
					
					stateMachine.states.ForEach(state =>
					{
						dropdownList.Add($"{i}:{layer.name}/{state.state.name}",new AnimationData()
						{
							LayerIndex = i,
							AnimationStateID = state.state.nameHash
						});
					});
				}
			}
			return dropdownList;
		}
		 
		#endif
		
		[Serializable]
		public struct AnimatorParameter
		{
			public AnimatorControllerParameterType Type;
			public int ParameterHash;
		}
		[Serializable]
		public struct AnimationData
		{
			public int LayerIndex;
			public int AnimationStateID;
		}
	}
}