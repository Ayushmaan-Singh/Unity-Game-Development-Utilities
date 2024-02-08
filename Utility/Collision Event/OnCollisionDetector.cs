using UnityEngine.Events;
using UnityEngine;

namespace AstekUtility
{
    public class OnCollisionDetector : MonoBehaviour
    {
        public UnityEvent<Collision> CollisionEnter;
        public UnityEvent<Collision> CollisionStay;
        public UnityEvent<Collision> CollisionExit;

        private void OnCollisionEnter(Collision collision)
        {
            CollisionEnter?.Invoke(collision);
        }

        private void OnCollisionStay(Collision collision)
        {
            CollisionStay?.Invoke(collision);
        }

        private void OnCollisionExit(Collision collision)
        {
            CollisionExit?.Invoke(collision);
        }
    }
}