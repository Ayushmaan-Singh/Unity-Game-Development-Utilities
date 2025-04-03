using Sirenix.OdinInspector;
using UnityEngine;

namespace AstekUtility.Gameplay
{
	public sealed class ContextSteering : MonoBehaviour
	{
		[Header("Context Steering Data")]
		[SerializeField]
		private ContextSolver contextSolver;
		[SerializeField]
		private Detector[] detectors;
		[SerializeField]
		private SteeringBehaviour[] steeringBehaviours;

		[Space]
		[SerializeField, MaxValue(360)]
		private int dirMapResolution = 16;

		[Header("Main Model Data")]
		[SerializeField]
		private Transform mainModel;
		[SerializeField]
		private Collider[] colliders;

		private AIData _aiData;
		private Vector3[] directionXZ;
		public Vector3 Direction { get; private set; } = Vector3.zero;

		private void Awake()
		{
			Init();
		}

		private void Init()
		{
			_aiData = new AIData();
			InitDirections();
			contextSolver = new ContextSolver(directionXZ, mainModel);

			foreach (SteeringBehaviour behavior in steeringBehaviours)
			{
				new SteeringBehaviour.Builder()
					.InitAIData(_aiData)
					.InitMainModel(mainModel)
					.InitDirectionXZ(directionXZ)
					.Build(behavior);
			}
			foreach (Detector detector in this.detectors)
			{
				new Detector.Builder()
					.InitAIData(_aiData)
					.InitMainModel(mainModel)
					.Build(detector);
			}
		}

		private void InitDirections()
		{
			directionXZ = new Vector3[dirMapResolution];
			float directionInterval = Mathf.PI * 2 / dirMapResolution;
			for (int i = 0; i < dirMapResolution; i++)
			{
				float currentAngle = i * directionInterval;
				directionXZ[i] = new Vector3(Mathf.Cos(currentAngle), 0, Mathf.Sin(currentAngle));
			}
		}

		private void FixedUpdate()
		{
			Direction = UpdateDirection();
		}

		private Vector3 UpdateDirection()
		{
			foreach (Detector detector in detectors)
			{
				detector.Detect();
			}

			if (_aiData.CurrentTarget)
			{
				if (!_aiData.CurrentTarget)
				{
					//Stopping Logic
					return Vector3.zero;
				}
				return contextSolver.GetDirectionToMove(steeringBehaviours);
			}
			if (_aiData.GetTargetsCount() > 0)
			{
				//Target acquisition logic
				_aiData.CurrentTarget = _aiData.Targets[0];
			}

			return Vector3.zero;
		}

		#if UNITY_EDITOR
		private void OnDrawGizmos()
		{
			contextSolver.OnDrawGizmos();
		}
		#endif
	}
}