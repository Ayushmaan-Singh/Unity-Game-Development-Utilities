using UnityEngine;
using UnityEngine.Events;

namespace Astek.SOAP
{
	public class IntVariable : RuntimeScriptableObject
	{
		[SerializeField] private int initialValue;
		[SerializeField] private int value;

		public event UnityAction<int> OnValueChanged = delegate { };

		public int Value
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