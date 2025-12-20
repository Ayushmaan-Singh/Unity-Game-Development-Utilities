using AstekUtility.DesignPattern.GenericPiplelineChains;
using UnityEngine;
namespace AstekUtility.Gameplay
{
    public class SetPosition : IProcessor<Transform, Transform>
    {
        private Vector3 _position;
        
        public SetPosition(Vector3 position)
        {
            _position = position;
        }

        public Transform Process(Transform input)
        {
            input.position = _position;
            return input;
        }
    }
    public class SetRotation_Quaternion : IProcessor<Transform, Transform>
    {
        private Quaternion _rotation;
        
        public SetRotation_Quaternion(Quaternion rotation)
        {
            _rotation = rotation;
        }

        public Transform Process(Transform input)
        {
            input.rotation = _rotation;
            return input;
        }
    }
    public class SetRotation_Euler : IProcessor<Transform, Transform>
    {
        private Vector3 _rotationEuler;

        public SetRotation_Euler(Vector3 rotationEuler)
        {
            _rotationEuler = rotationEuler;
        }

        public Transform Process(Transform input)
        {
            input.rotation = Quaternion.Euler(_rotationEuler);
            return input;
        }
    }
    public class SetScale : IProcessor<Transform, Transform>
    {
        private Vector3 _scale;
        public SetScale(Vector3 scale)
        {
            _scale = scale;
        }
        public Transform Process(Transform input)
        {
            input.localScale = _scale;
            return input;
        }
    }
}