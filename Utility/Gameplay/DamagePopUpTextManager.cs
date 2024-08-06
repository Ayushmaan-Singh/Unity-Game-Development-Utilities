using System.Threading;
using System.Threading.Tasks;
using AstekUtility.ServiceLocatorTool;
using Combat;
using Functional;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
namespace AstekUtility.Gameplay
{
	public class DamagePopUpTextManager : MonoBehaviour, IGameService
	{

		[SerializeField] private Transform _popuptextPrefab;
		[SerializeField] private int _poolSize = 60;

		[Title("Pop Up Sequence")]
		[SerializeField] private float _displacementAmount;
		[SerializeField] private float _fontSizeIncrease = 7;

		[SerializeField] private AnimationCurve _displacementCurve;
		[SerializeField] private AnimationCurve _sizeLerp;
		[SerializeField] private AnimationCurve _alphaLerp;

		[SerializeField] [MinValue(0.1f)]
		private float _effectDuration;
		[SerializeField] [Tooltip("Higher value give faster iteration")] [Range(0.01f, 1f)]
		private float _stepSize = 0.1f;

		[Header("Popup Color")]
		[SerializeField] private Color physicalNormal;
		[SerializeField] private Color physicalCrit;
		[SerializeField] private Color enerbyBasedNormal;
		[SerializeField] private Color energyBasedCrit;

		private CancellationTokenSource _cts;

		private ObjectPool<Transform> _popuptextPool;
		private int _popuptextSpawned;
		public EntityState CurrentState { get; set; } = EntityState.Running;

		private void Awake()
		{
			_popuptextPool = new ObjectPool<Transform>((uint)_poolSize);
		}

		private void OnEnable()
		{
			ServiceLocator.Instance.Register(this);
			CurrentState = EntityState.Running;
			_cts = new CancellationTokenSource();
		}

		private void OnDisable()
		{
			_cts.Cancel();
			ServiceLocator.Instance.Unregister<DamagePopUpTextManager>();
		}

		public void Execute(Vector3 startPoint, Vector3 direction, DamageType dmgType, DamageIntensity dmgIntensity, float dmgAmount)
		{
			PopuptextSequence(startPoint, dmgType, dmgIntensity, dmgAmount, direction);
		}
		public void Execute(Vector3 startPoint, DamageType dmgType, DamageIntensity dmgIntensity, float dmgAmount)
		{
			PopuptextSequence(startPoint, dmgType, dmgIntensity, dmgAmount);
		}

		/// <summary>
		///     Popuptext moves in a paraboloc motion in direction or vertical up if no direction provided
		/// </summary>
		/// <param name="startPosition"></param>
		/// <param name="direction"></param>
		/// <param name="dmgIntensity"></param>
		/// <param name="damageAmount"></param>
		private async void PopuptextSequence(Vector3 startPosition, DamageType dmgType, DamageIntensity dmgIntensity, float damageAmount, Vector3 direction = default(Vector3))
		{
			Transform popuptext = GetPopuptext();
			RectTransform popuptextTransform = popuptext.GetComponent<RectTransform>();
			TextMeshPro popuptextMesh = popuptext.GetComponent<TextMeshPro>();

			if (popuptext == null)
				return;

			popuptextMesh.text = $"{damageAmount}";

			//Set color depending on damage type and damage intensity
			switch (dmgType)
			{
				case DamageType.Physical:

					switch (dmgIntensity)
					{
						case DamageIntensity.Normal:
							popuptextMesh.color = new Color(physicalNormal.r, physicalNormal.g, physicalNormal.b, 0);
							break;
						case DamageIntensity.CritDamage:
							popuptextMesh.color = new Color(physicalCrit.r, physicalCrit.g, physicalCrit.b, 0);
							break;
					}
					break;

				case DamageType.EnergyBased:

					switch (dmgIntensity)
					{
						case DamageIntensity.Normal:
							popuptextMesh.color = new Color(enerbyBasedNormal.r, enerbyBasedNormal.g, enerbyBasedNormal.b, 0);
							break;
						case DamageIntensity.CritDamage:
							popuptextMesh.color = new Color(energyBasedCrit.r, energyBasedCrit.g, energyBasedCrit.b, 0);
							break;
					}
					break;

				default:
					Debug.LogError("Incorrect damage type passed");
					break;
			}

			popuptext.gameObject.SetActive(true);

			Vector3 finalPosition;
			if (direction != default(Vector3))
				finalPosition = startPosition + direction.normalized * _displacementAmount;
			else
				finalPosition = startPosition + new Vector3(1, 0, 1) * _displacementAmount;
			float baseFontSize = popuptextMesh.fontSize;

			//Perform Visual Action
			for (float i = 0; i <= 1; i += _stepSize)
			{
				if (_cts.Token.IsCancellationRequested)
					return;

				float alphaValue = _alphaLerp.Evaluate(i) * 255;
				float fontSize = _sizeLerp.Evaluate(i) * _fontSizeIncrease;

				popuptextMesh.color = new Color(popuptextMesh.color.r, popuptextMesh.color.g, popuptextMesh.color.b, alphaValue);
				popuptextMesh.fontSize = baseFontSize + fontSize;

				if (direction != default(Vector3))
				{
					Vector3 pivot = (startPosition + finalPosition) * 0.5f - new Vector3(0, 0, 0.01f);
					popuptextTransform.position = Vector3.Slerp(startPosition - pivot, finalPosition - pivot, _displacementCurve.Evaluate(i));
					popuptextTransform.position += pivot;
				}
				else
				{
					popuptextTransform.position = Vector3.Lerp(startPosition, finalPosition, _displacementCurve.Evaluate(i));
				}

				while (CurrentState == EntityState.Paused)
				{
					await Task.Delay(100);
				}
				await Task.Delay((int)(_effectDuration * 1000 * _stepSize));
			}

			//Sequence End
			popuptextMesh.fontSize = baseFontSize;
			popuptext.gameObject.SetActive(false);
			_popuptextPool.AddItemToPool(popuptext);
		}

		private Transform GetPopuptext()
		{
			Transform popuptext = _popuptextPool.GetItemFromPool();
			if (popuptext == null && _popuptextSpawned < _poolSize)
			{
				popuptext = Instantiate(_popuptextPrefab, transform);
			}

			return popuptext;
		}

		private void PoolPopupText(Transform objectToPool)
		{
			_popuptextPool.AddItemToPool(objectToPool);
		}
	}
}