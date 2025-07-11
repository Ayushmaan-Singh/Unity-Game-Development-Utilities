using UnityEngine;
using UnityEngine.Events;

namespace AstekUtility
{
	public class OnCollisionExit2DEvent : MonoBehaviour
	{
		[SerializeField] private UnityEvent<Collision2D> onCollisionExit;

		private void OnCollisionExit2D(Collision2D other) => onCollisionExit?.Invoke(other);

		
		public void Register(UnityAction<Collision2D> action) => onCollisionExit.AddListener(action);

		public void Deregister(UnityAction<Collision2D> action) => onCollisionExit.AddListener(action);

	}
}