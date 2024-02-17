using System;
using System.Collections.Generic;
using UnityEngine;

namespace AstekUtility.Observer
{
    public sealed class ObserverManager
    {
        public static ObserverManager Instance { get; private set; } = new ObserverManager();

        /// <summary>
        /// Key is the instance of subject
        /// Value is the List of observer observing it
        /// </summary>
        private readonly Dictionary<Type, List<IObserver>> _subjectAndObservers = new Dictionary<Type, List<IObserver>>();
        private readonly Dictionary<Type, ISubject> _subjectTypeToInstance = new Dictionary<Type, ISubject>();

        /// <summary>
        /// Prevents allocation as instantiated class
        /// </summary>
        private ObserverManager() { }

        /// <summary>
        /// Register a new subject to the collection
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="subject"></param>
        public void RegisterSubject<T>(T subject) where T : ISubject
        {
            if (_subjectAndObservers.ContainsKey(typeof(T)))
            {
                Debug.LogError($"Observer Manager: Attempted to add subject of type {typeof(T).Name} which is already added.");
                return;
            }

            _subjectAndObservers.Add(typeof(T), new List<IObserver>());
            _subjectTypeToInstance.Add(typeof(T), subject);
        }

        /// <summary>
        /// Unregister a exsisting subject from collection
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="subject"></param>
        public void UnregisterSubject<T>() where T : ISubject
        {
            if (!_subjectAndObservers.ContainsKey(typeof(T)))
            {
                Debug.LogError($"Observer Manager: Attempted to remove subject of type {typeof(T).Name} which is not registered.");
                return;
            }

            _subjectAndObservers.Remove(typeof(T));
            _subjectTypeToInstance.Remove(typeof(T));
        }

        /// <summary>
        /// Subscribe an Observer of type W to Subject of type T
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="W"></typeparam>
        /// <param name="subject"></param>
        /// <param name="observer"></param>
        public void SubscribeToSubject<T, W>(T subject, W observer) where T : ISubject where W : IObserver
        {
            if (!_subjectAndObservers.ContainsKey(subject.GetType()))
            {
                RegisterSubject(subject);
            }

            if (_subjectAndObservers[typeof(T)].Contains(observer))
            {
                Debug.LogError($"Observer {observer.GetType().Name} marked for subscription to Subject {subject.GetType().Name} is already subscribed to it");
            }

            _subjectAndObservers[typeof(T)].Add(observer);
        }

        /// <summary>
        /// Unsubscribe an Observer of type W to Subject of type T
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="W"></typeparam>
        /// <param name="subject"></param>
        /// <param name="observer"></param>
        public void UnsubscribeToSubject<T, W>(T subject,W observer) where T : ISubject where W : IObserver
        {
            if (!_subjectAndObservers.ContainsKey(subject.GetType()))
            {
                Debug.LogError($"Subject {typeof(T).Name} is not registered");
            }
            else if (!_subjectAndObservers[typeof(T)].Contains(observer))
            {
                Debug.LogError($"Observer {observer.GetType().Name} does not subscribe to Subject {typeof(T).Name}");
            }

            _subjectAndObservers[typeof(T)].Remove(observer);
        }

        /// <summary>
        /// Notify Observers Of Change
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="subject"></param>
        public void NotifyObservers<T>(T subject) where T : ISubject
        {
            foreach (IObserver observer in _subjectAndObservers[subject.GetType()])
            {
                observer.OnNotify(_subjectTypeToInstance[typeof(T)]);
            }
        }

        /// <summary>
        /// Unregister all subjects
        /// usually done when game is closed
        /// </summary>
        public void UnregisterAll()
        {
            _subjectAndObservers.Clear();
        }

        /// <summary>
        /// Checks if a subject is registered
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public bool CheckIfRegistered<T>() where T : ISubject
        {
            if (_subjectAndObservers.ContainsKey(typeof(T)))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

#if UNITY_EDITOR

        /// <summary>
        /// This is Used so that static variables are refreshed once the play mode is over when having domain reload disabled
        /// </summary>
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        public static void ResetStatic()
        {
            Instance = new ObserverManager();
        }

#endif
    }
}
