using UnityEngine;

/// <summary>
/// Abstract base class for creating a Singleton pattern in Unity.
/// Ensures that only one instance of the derived class exists throughout the game.
/// </summary>
/// <typeparam name="T">The type of MonoBehaviour to be made into a singleton.</typeparam>
public abstract class Singleton<T> : MonoBehaviour where T : MonoBehaviour
{
    /// <summary>
    /// Static variable holding the single instance of the Singleton.
    /// </summary>
    private static T _instance;

    /// <summary>
    /// Lock object for thread safety when accessing the instance.
    /// </summary>
    private static readonly object _lock = new object();

    /// <summary>
    /// Provides a global access point to the singleton instance.
    /// </summary>
    public static T Instance
    {
        get
        {
            lock (_lock)
            {
                if (_instance == null)
                {
                    // Try to find an existing instance in the scene
                    _instance = FindObjectOfType<T>();
                    if (_instance == null)
                    {
                        // If no instance is found, create a new one
                        GameObject singletonObject = new GameObject();
                        _instance = singletonObject.AddComponent<T>();
                        singletonObject.name = typeof(T).ToString() + " (Singleton)";
                    }
                }
                return _instance;
            }
        }
    }

    /// <summary>
    /// Called before the first frame update.
    /// Ensures that only one instance of the Singleton exists and destroys any duplicates.
    /// </summary>
    protected virtual void Awake()
    {
        if (_instance == null)
        {
            // If no instance is set, this object is the singleton
            _instance = this as T;
            DontDestroyOnLoad(gameObject); // Ensure the singleton persists across scenes
        }
        else
        {
            // Destroy any duplicate instances found
            Destroy(gameObject);
        }
    }
}