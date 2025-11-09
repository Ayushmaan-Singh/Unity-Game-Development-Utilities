using System;
using AstekUtility;
using UnityEngine;
using UnityEngine.UIElements;
namespace Gameplay
{
	/// <summary>
	/// Currently it only handles 
	/// </summary>
	[RequireComponent(typeof(ParticleSystem))]
	public class TrailParticleSys : MonoBehaviour
	{
		[SerializeField] private float particlePerUnitDistance = 0.5f;
		[SerializeField] private float rotationOffset;
		private ParticleSystem _particleSystem;

		private Vector2 _velocity = Vector2.zero;
		private Vector2 _prevPosition;
		private float _distanceCounter;
		private float _spawnParticleEveryDistance;

		private void Awake()
		{
			_particleSystem = GetComponent<ParticleSystem>();
			_spawnParticleEveryDistance = (1f / particlePerUnitDistance).SetPrecision(4);
		}
		public void OnObjectMove(Vector2 position)
		{
			_velocity = (position - _prevPosition).normalized;
			_distanceCounter += (_prevPosition.SqrMagnitudeDistance(position)).SetPrecision(4);
			while (_distanceCounter >= _spawnParticleEveryDistance)
			{
				EmitWithVelocity(position - (_velocity * (_distanceCounter - _spawnParticleEveryDistance).SetPrecision(4)));
				_distanceCounter = (_distanceCounter - _spawnParticleEveryDistance).SetPrecision(4);
			}
			_prevPosition = position;
		}
		private void EmitWithVelocity(Vector3 position)
		{
			ParticleSystem.EmitParams emitParams = new ParticleSystem.EmitParams();
			//If alignment to Up
			float angleZ = Mathf.Atan2(_velocity.x, _velocity.y) * Mathf.Rad2Deg + rotationOffset;
			emitParams.rotation3D = new Vector3(0, 0, angleZ);
			emitParams.position = position;

			// Emit one particle
			_particleSystem.Emit(emitParams, 1);
		}

		#region Setters

		public TrailParticleSys Set_StartPosition(Vector3 position)
		{
			_prevPosition = position;
			return this;
		}

		#endregion
	}
}