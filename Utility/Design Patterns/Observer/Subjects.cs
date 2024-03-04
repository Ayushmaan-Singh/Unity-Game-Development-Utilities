using System.Collections.Generic;

namespace AstekUtility.Observer
{
    namespace Managed
    {
        public interface ISubject { }

        public class SubjectBase<T> : ISubject
        {
            public T Data { get; set; }
        }
    }

    namespace Unmanaged
    {
        public interface ISubject<T>
        {
            void Attach(params IObserver<T>[] observer);
            void Detach(params IObserver<T>[] observer);
            void Notify();
        }

        public class Subject<T> : ISubject<T>
        {
            private List<IObserver<T>> _observers = new List<IObserver<T>>();
            public T Value { get; set; }

            public void Attach(params IObserver<T>[] observer)
            {
                foreach (IObserver<T> obs in observer)
                {
                    if (!_observers.Contains(obs))
                    {
                        _observers.Add(obs);
                    }
                }
            }

            public void Detach(params IObserver<T>[] observer)
            {
                foreach (IObserver<T> obs in observer)
                {
                    if (_observers.Contains(obs))
                    {
                        _observers.Remove(obs);
                    }
                }
            }

            public void DetachAll()
            {
                _observers.Clear();
            }

            public void Notify()
            {
                foreach (IObserver<T> obs in _observers)
                {
                    obs.OnNotify(this);
                }
            }
        }
    }

}
