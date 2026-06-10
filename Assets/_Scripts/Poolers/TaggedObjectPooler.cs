using System.Collections.Generic;
using UnityEngine;

namespace _Scripts.Poolers
{
    public class TaggedObjectPooler : Singleton<TaggedObjectPooler>
    {
        [System.Serializable]
        public class Pool
        {
            public string tag;  // The tag to identify the pool.
            public GameObject prefab;  // The prefab to pool.
            public int initialPoolSize = 10;  // Initial number of objects in the pool.
        }

        public int defaultPoolSize = 240;
        public List<Pool> pools;  // A list of different pools.

        private Dictionary<string, Queue<GameObject>> pooledObjects;  // A dictionary to hold pooled objects by tags.

        void Start()
        {
            pooledObjects = new Dictionary<string, Queue<GameObject>>();

            foreach (Pool pool in pools)
            {
                pooledObjects[pool.tag] = new Queue<GameObject>();

                for (int i = 0; i < pool.initialPoolSize; i++)
                {
                    GameObject obj = Instantiate(pool.prefab);
                    obj.SetActive(false);  // Deactivate initially to save resources.
                    pooledObjects[pool.tag].Enqueue(obj);
                }
            }
        }

        public GameObject GetPooledObject(string tag)
        {
            if (pooledObjects.ContainsKey(tag) && pooledObjects[tag].Count > 0)
            {
                GameObject obj = pooledObjects[tag].Dequeue();
                obj.SetActive(true);  // Activate the object when retrieved.
                return obj;
            }
    
            // If no objects are available, instantiate a new one.
            Pool pool = pools.Find(p => p.tag == tag);
            if (pool != null)
            {
                GameObject newObj = Instantiate(pool.prefab);
                return newObj;
            }

            Debug.LogWarning($"No pool found for tag: {tag}");
            return null;
        }

        public void ReturnObject(GameObject obj, string tag)
        {
            if (obj != null && pooledObjects.ContainsKey(tag))
            {
                obj.SetActive(false);  // Deactivate the object when returned to pool.
                pooledObjects[tag].Enqueue(obj);
            }
        }
        
        public GameObject GetPooledObject(string tag, GameObject prefab = null)
        {
            if (!pooledObjects.ContainsKey(tag) && prefab != null)
            {
                CreatePool(tag, prefab);
            }
            
            if (pooledObjects.ContainsKey(tag) && pooledObjects[tag].Count > 0)
            {
                GameObject obj = pooledObjects[tag].Dequeue();
                obj.SetActive(true);  // Activate the object when retrieved.
                return obj;
            }
    
            // If no objects are available, instantiate a new one.
            Pool pool = pools.Find(p => p.tag == tag);
            if (pool != null)
            {
                GameObject newObj = Instantiate(pool.prefab);
                return newObj;
            }

            Debug.LogWarning($"No pool found for tag: {tag}");
            return null;
        }

        private void CreatePool(string tag, GameObject prefab)
        {
            pooledObjects[tag] = new Queue<GameObject>();

            for (int i = 0; i < defaultPoolSize; i++)
            {
                GameObject obj = Instantiate(prefab);
                obj.SetActive(false);  // Deactivate initially to save resources.
                pooledObjects[tag].Enqueue(obj);
            }
        }
    }
}