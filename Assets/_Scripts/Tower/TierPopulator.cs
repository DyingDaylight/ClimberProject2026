using System.Collections.Generic;
using _Scripts.Parameters;
using _Scripts.Poolers;
using UnityEngine;

public class TierPopulator : MonoBehaviour
{
    [SerializeField] private FaceController[] faces;
    private List<Poolable> poolableObjects = new List<Poolable>();
    
    public void AddLaddersToFace(int faceIndex, bool[] ladderSlots, TierParameters tierParameters)
    {
        if (faces == null || faceIndex < 0 || faceIndex >= faces.Length)
            return;
        
        FaceController face = faces[faceIndex];

        for (int i = 0; i < ladderSlots.Length; i++)
        {
            if (!ladderSlots[i])
                continue;
            
            GameObject ladder = TaggedObjectPooler.Instance.GetPooledObject("Ladder");
            
            Transform slot = i < 3 ? face.GetUpperSlot(i) : face.GetLowerSlot(i % 3);
            
            ladder.transform.SetParent(slot, false);
            ladder.transform.localPosition  = Vector3.zero;
            ladder.transform.localRotation = Quaternion.identity;
            ladder.transform.localScale = Vector3.one;
            
            
            Poolable poolable = ladder.GetComponentInChildren<Poolable>();
            if (poolable == null)
                Debug.LogError("Ladder is not poolable");
            else
                poolableObjects.Add(poolable);

            if (i < 3)
                TrySpawnCollectables(slot, tierParameters);
        }
    }

    private void TrySpawnCollectables(Transform slot, TierParameters tierParameters)
    {
        float val = Random.value;
        if (val <= tierParameters.collectableSpawnChance)
        {
            Collectable selected = PickWeighted(tierParameters.collectables);
            GameObject collectable = TaggedObjectPooler.Instance.GetPooledObject(selected.tag, selected.collectablePrefab);
            if (!collectable)
                return;
            
            //collectable.transform.SetParent(slot, false);
            collectable.transform.SetParent(null);
            //collectable.transform.localPosition = new Vector3(0f, 0f, 0f);
            collectable.transform.position = slot.TransformPoint(new Vector3(0f, 0f, -0.1f));
            collectable.transform.rotation = Quaternion.identity;
            collectable.transform.localScale = Vector3.one;
            
            Poolable poolable = collectable.GetComponentInChildren<Poolable>();
            if (poolable == null)
                Debug.LogError("Collectable is not poolable");
            else
                poolableObjects.Add(poolable);
        }
    }

    public void ClearObjects()
    {
        foreach (Poolable obj in poolableObjects)
        {
            obj.ReturnToPool();
        }
        
        poolableObjects.Clear();
    }
    
    private Collectable PickWeighted(List<Collectable> collectables)
    {
        float totalWeight = 0f;

        foreach (var collectible in collectables)
            totalWeight += collectible.chance;

        float roll = Random.Range(0f, totalWeight);

        foreach (var collectible in collectables)
        {
            roll -= collectible.chance;

            if (roll <= 0f)
                return collectible;
        }

        return collectables[^1];
    }
}
