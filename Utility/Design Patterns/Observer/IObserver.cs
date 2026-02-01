namespace Astek.Observer
{
	namespace Managed
	{
		public interface IObserver
		{
			void OnNotify(ISubject subject);
		}
	}

	namespace Unmanaged
	{
		public interface IObserver<T>
		{
			void OnNotify(ISubject<T> subject);
		}
	}
}