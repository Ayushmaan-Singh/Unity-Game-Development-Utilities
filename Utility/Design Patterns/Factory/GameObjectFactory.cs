using System;
using UnityEngine;

namespace AstekUtility
{
    /// <summary>
    /// This is a scriptable obj event to give ability to any script to use pooled factory for instantiating
    /// </summary>
    public class GameObjectFactory : ScriptableObject, IFactory<GameObject>
    {
        public ObjectPool<GameObject> GameObjectPool = new ObjectPool<GameObject>();

        /// <summary>
        /// This delegate gives condition for checking which obj is required from the pool
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public delegate bool MatchingCondition(GameObject obj);

        public GameObject CreateInstance(GameObject prefab)
        {
            return GameObject.Instantiate(prefab);
        }

        public GameObject CreateInstance(GameObject prefab, MatchingCondition condition)
        {
            //Try Getting obj from pool
            for (int i = 0; i < GameObjectPool.GetPooledItemCount(); i++)
            {
                GameObject item = GameObjectPool.GetItemFromPool();

                //We check if we get any obj from pool and if we get we match it with condition to check if this is the right one
                if (item != null)
                {
                    if (condition(item))
                        return item;
                    else
                        GameObjectPool.AddItemToPool(item);
                }
            }

            //Instantiate it
            return CreateInstance(prefab);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj">GameObject to destroy</param>
        /// <param name="trueDestroy">if true then obj is destroyed else sent to pool</param>
        /// <param name="immediate">destroy immedeate or not</param>
        public void Destroy(GameObject obj, bool trueDestroy = false, bool immediate = false)
        {
            if(trueDestroy)
            {
                if(!immediate)
                {
                    Destroy(obj);
                }
                else
                {
                    DestroyImmediate(obj);
                }
            }
            else
            {
                GameObjectPool.AddItemToPool(obj);
            }
        }
    }
}