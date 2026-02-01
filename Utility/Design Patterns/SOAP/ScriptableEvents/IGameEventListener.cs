namespace Astek.SOAP.ScriptableEvents
{
	public interface IGameEventListener<T>
	{
		void OnEventRaised(T data);
	}
	
	
}