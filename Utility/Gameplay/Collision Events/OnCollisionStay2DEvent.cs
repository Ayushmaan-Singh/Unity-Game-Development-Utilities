using UnityEngine;
using UnityEngine.Events;

namespace AstekUtility
{
	public class OnCollisionStay2DEvent : MonoBehaviour
	{
		[SerializeField] private UnityEvent<Collision2D> onCollisionStay;

		private void OnCollisionStay2D(Collision2D other) => onCollisionStay?.Invoke(other);

		
		public void Register(UnityAction<Collision2D> action) => onCollisionStay.AddListener(action);

		public void Deregister(UnityAction<Collision2D> action) => onCollisionStay.AddListener(action);

	}
}