using System;
using System.Collections.Generic;
using System.Linq;
using AstekUtility.CardinalDirection;
using UnityEngine;
namespace AstekUtility.Gameplay
{
	[Serializable]
	public abstract class ContextSteering
	{
		[field:SerializeField] public ContextSolver ContextSolver { get; private set; }
		protected AIData _aiData;
		protected List<Detector> _detectors = new List<Detector>();

		protected Vector3[] _direction8SidesXZ;
		protected Transform _mainModel;
		protected List<SteeringBehaviour> _steeringBehaviours = new List<SteeringBehaviour>();

		public ContextSteering(List<SteeringBehaviour> steeringBehaviours, List<Detector> detectors, Transform mainModel)
		{
			_steeringBehaviours = steeringBehaviours;
			_detectors = detectors;
			_mainModel = mainModel;

			_aiData = new AIData();
			_direction8SidesXZ = new Vector3[8];
			IEnumerable<Direction8Sides> directions = Enum.GetValues(typeof(Direction8Sides)).Cast<Direction8Sides>();

			int i = 0;
			foreach (Direction8Sides dir in directions)
			{
				Vector2Int dirXZ = DirectionHelper8Sides.GetVectorinDirection(dir);
				_direction8SidesXZ[i] = new Vector3(dirXZ.x, 0, dirXZ.y);
				i++;
			}

			ContextSolver = new ContextSolver(_direction8SidesXZ, _mainModel);

			foreach (SteeringBehaviour behavior in _steeringBehaviours)
			{
				object[] parameters =
				{
					_mainModel, _direction8SidesXZ, _aiData
				};
				behavior.GetType().InvokeMethodByName(behavior, "Init", parameters);
			}
			foreach (Detector detector in _detectors)
			{
				object[] parameters =
				{
					_mainModel, _aiData
				};
				detector.GetType().InvokeMethodByName(detector, "Init", parameters);
			}
		}

		public Vector3 UpdatePosition()
		{
			foreach (Detector detector in _detectors)
			{
				detector.Detect();
			}

			if (_aiData.currentTarget != null)
			{
				if (_aiData.currentTarget == null)
				{
					//Stopping Logic
					return Vector3.zero;
				}
				return ContextSolver.GetDirectionToMove(_steeringBehaviours);
			}
			if (_aiData.GetTargetsCount() > 0)
			{
				//Target acquisition logic
				_aiData.currentTarget = _aiData.Targets[0];
			}

			return Vector3.zero;
		}
	}
}