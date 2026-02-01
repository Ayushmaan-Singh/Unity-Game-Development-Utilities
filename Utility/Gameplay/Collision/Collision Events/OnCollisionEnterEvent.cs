using UnityEngine;
using UnityEngine.Events;

namespace Astek
{
	public class OnCollisionEnterEvent : MonoBehaviour
	{
		[SerializeField] private UnityEvent<Collision> onCollisionEnter;

		private void OnCollisionEnter(Collision other) => onCollisionEnter?.Invoke(other);


		public void Register(UnityAction<Collision> action) => onCollisionEnter.AddListener(action);

		public void Deregister(UnityAction<Collision> action) => onCollisionEnter.AddListener(action);

	}
}