using UnityEngine;

namespace _Scripts.Parameters
{
    [CreateAssetMenu(fileName = "Collectable", menuName = "Collectable", order = 0)]
    public class Collectable : ScriptableObject
    {
        public string tag;
        public GameObject collectablePrefab;
        [Range(0, 1)] public float chance;
        
    }
}