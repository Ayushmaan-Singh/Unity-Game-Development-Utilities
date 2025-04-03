using UnityEngine;
using UnityEngine.Events;
namespace AstekUtility.SOAP
{
	public class FloatVariable : RuntimeScriptableObject
	{
		[SerializeField] private float initialValue;
		[SerializeField] private float value;

		public event UnityAction<float> OnValueChanged = delegate { };

		public float Value
		{
			get => value;
			set
			{
				if (this.value == value) return;
				this.value = value;
				OnValueChanged.Invoke(this.value);
			}
		}
		
		protected override void OnReset() => OnValueChanged.Invoke(initialValue);
	}
}