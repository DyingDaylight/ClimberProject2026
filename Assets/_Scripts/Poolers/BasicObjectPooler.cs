using System.Collections.Generic;
using UnityEngine;

namespace _Scripts.Poolers
{
    public class BasicObjectPooler : MonoBehaviour
    {
        public GameObject prefab;  // The prefab to pool.
        public int initialPoolSize = 10;  // Initial number of objects in the pool.
    
        private Queue<GameObject> pooledObjects;  // A queue to hold pooled objects.

        void Start()
        {
            pooledObjects = new Queue<GameObject>();
        
            // Populate the pool with initial objects.
            for (int i = 0; i < initialPoolSize; i++)
            {
                GameObject obj = Instantiate(prefab);
                obj.SetActive(false);  // Deactivate initially to save resources.
                pooledObjects.Enqueue(obj);
            }
        }

        public GameObject GetPooledObject()
        {
            if (pooledObjects.Count > 0)
            {
                GameObject obj = pooledObjects.Dequeue();
                obj.SetActive(true);  // Activate the object when retrieved.
                return obj;
            }
        
            // If no objects are available, instantiate a new one.
            GameObject newObj = Instantiate(prefab);
            return newObj;
        }

        public void ReturnObject(GameObject obj)
        {
            if (obj != null)
            {
                obj.SetActive(false);  // Deactivate the object when returned to pool.
                pooledObjects.Enqueue(obj);
            }
        }
    }
}