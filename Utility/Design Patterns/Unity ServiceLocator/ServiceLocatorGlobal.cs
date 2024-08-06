using UnityEngine;
namespace AstekUtility.DesignPattern.ServiceLocatorTool
{
	[AddComponentMenu("ServiceLocator/ServiceLocator Global")]
	public class ServiceLocatorGlobal : Bootstrapper
	{
		[SerializeField] bool dontDestroyOnLoad = true;

		protected override void Bootstrap()
		{
			Container.ConfigureAsGlobal(dontDestroyOnLoad);
		}
	}
}