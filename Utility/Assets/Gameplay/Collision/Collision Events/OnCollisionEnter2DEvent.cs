using UnityEngine;
using UnityEngine.Events;

namespace Astek
{
	public class OnCollisionEnter2DEvent : MonoBehaviour
	{
		[SerializeField] private UnityEvent<Collision2D> onCollisionEnter;

		private void OnCollisionEnter2D(Collision2D other) => onCollisionEnter?.Invoke(other);

		
		public void Register(UnityAction<Collision2D> action) => onCollisionEnter.AddListener(action);

		public void Deregister(UnityAction<Collision2D> action) => onCollisionEnter.AddListener(action);

	}
}