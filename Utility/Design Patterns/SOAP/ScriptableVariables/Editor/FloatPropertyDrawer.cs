using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;
using ObjectField = UnityEditor.Search.ObjectField;
namespace Astek.SOAP.Editor
{
	[CustomPropertyDrawer(typeof(FloatVariable))]
	public class FloatPropertyDrawer : PropertyDrawer
	{
		public override VisualElement CreatePropertyGUI(SerializedProperty property)
		{
			VisualElement container = new VisualElement();
			ObjectField objectField = new ObjectField(property.displayName)
			{
				objectType = typeof(FloatVariable)
			};
			objectField.BindProperty(property);

			Label valueLabel = new Label();
			valueLabel.style.paddingLeft = 20;

			container.Add(objectField);
			container.Add(valueLabel);

			objectField.RegisterValueChangedCallback(
				evt =>
				{
					FloatVariable variable = evt.newValue as FloatVariable;
					if (variable != null)
					{
						valueLabel.text = $"Current Value:{variable.Value}";
						variable.OnValueChanged += newValue => valueLabel.text = $"Current Value: {newValue}";
					}
					else
						valueLabel.text = string.Empty;
				});

			FloatVariable currentVariable = property.objectReferenceValue as FloatVariable;
			if (currentVariable != null)
			{
				valueLabel.text = $"Current Value: {currentVariable.Value}";
				currentVariable.OnValueChanged += newValue => valueLabel.text = $"Current Value: {newValue}";
			}

			return container;
		}
	}
}