using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AstekUtility.EventSystem
{
    //Improve this later on to day,hour,min and sec
    public class TimedEventCounter : MonoBehaviour
    {
        private static TimedEventCounter instance = null;
        private bool keepTimeCounter = false;
        private float timeCounter;

        public static TimedEventCounter Instance { get => instance; }

        public delegate void EventDelegate();

        //Dictionary containg all the events that are being monitored for completion
        private Dictionary<float, EventDelegate> delegateDict;

        private void Awake()
        {
            delegateDict = new Dictionary<float, EventDelegate>();
            if (instance == null)
            {
                instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }

        private void Update()
        {
            timeCounter += Time.time;

            if (delegateDict.Count >= 1)
            {
                if (!keepTimeCounter)
                {
                    keepTimeCounter = true;
                    StartCoroutine(TimeCounterSys());
                }
            }
        }

        private IEnumerator TimeCounterSys()
        {
            while (true)
            {
                foreach (KeyValuePair<float, EventDelegate> evt in delegateDict)
                {
                    if (evt.Key <= Time.time)
                    {
                        evt.Value.Invoke();
                        delegateDict.Remove(evt.Key);

                        if (delegateDict.Count == 0)
                        {
                            keepTimeCounter = false;
                            yield break;
                        }
                    }
                }

                yield return null;
            }
        }

        public void AddDelegate(float time, EventDelegate eventDel)
        {
            if (delegateDict.ContainsKey(time))
                return;

            //This is the one that gets involked
            delegateDict.Add(time, eventDel);
        }

        public void AddListener(float time, EventDelegate evt)
        {
            if (delegateDict.ContainsKey(time))
            {
                delegateDict[time] += evt;
            }
        }

        public void RemoveAllListener()
        {
            delegateDict.Clear();
        }
    }
}
