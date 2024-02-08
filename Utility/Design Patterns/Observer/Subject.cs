using System.Collections.Generic;

namespace AstekUtility.Observer
{
    public class Subject<T> : ISubject
    {
        public T Data { get; set; }

        public void Attach(IObserver observer)
        {
            ObserverManager.Instance.SubscribeToSubject(this, observer);
        }

        public void Detach(IObserver observer)
        {
            ObserverManager.Instance.UnsubscribeToSubject(this, observer);
        }

        public void Notify()
        {
            ObserverManager.Instance.NotifyObservers(this);
        }
    }
}
