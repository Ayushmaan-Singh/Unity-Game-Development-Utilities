using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;
using ObjectField = UnityEditor.Search.ObjectField;

namespace AstekUtility.SOAP.Editor
{
	[CustomPropertyDrawer(typeof(IntVariable))]
	public class IntVariablePropertyDrawer : PropertyDrawer
	{
		public override VisualElement CreatePropertyGUI(SerializedProperty property)
		{
			VisualElement container = new VisualElement();
			ObjectField objectField = new ObjectField(property.displayName)
			{
				objectType = typeof(IntVariable)
			};
			objectField.BindProperty(property);

			Label valueLabel = new Label();
			valueLabel.style.paddingLeft = 20;

			container.Add(objectField);
			container.Add(valueLabel);

			objectField.RegisterValueChangedCallback(
				evt =>
				{
					IntVariable variable = evt.newValue as IntVariable;
					if (variable != null)
					{
						valueLabel.text = $"Current Value:{variable.Value}";
						variable.OnValueChanged += newValue => valueLabel.text = $"Current Value: {newValue}";
					}
					else
						valueLabel.text = string.Empty;
				});

			IntVariable currentVariable = property.objectReferenceValue as IntVariable;
			if (currentVariable != null)
			{
				valueLabel.text = $"Current Value: {currentVariable.Value}";
				currentVariable.OnValueChanged += newValue => valueLabel.text = $"Current Value: {newValue}";
			}

			return container;
		}
	}
}