using _Scripts.Poolers;
using UnityEngine;

public class Poolable : MonoBehaviour
{
    [SerializeField] private string poolTag;

    public string PoolTag => poolTag;

    public void ReturnToPool()
    {
        transform.parent = null;
        TaggedObjectPooler.Instance.ReturnObject(gameObject, poolTag);
    }
}
