using Sirenix.OdinInspector;
using UnityEngine;
using XNode;
namespace AstekUtility.VisualFeedback
{
	[DrawWithUnity]
	public class FeedbackGraph : NodeGraph
	{
		[field:SerializeField] public FeedbackStatus FeedbackStatus;
	}

	public enum FeedbackStatus
	{
		Running,
		Paused,
		Completed
	}
}