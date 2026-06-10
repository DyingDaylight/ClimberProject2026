using System.Collections.Generic;
using _Scripts.Parameters;
using _Scripts.Poolers;
using UnityEngine;

public class TierPopulator : MonoBehaviour
{
    [SerializeField] private FaceController[] faces;
    private List<GameObject> objects = new List<GameObject>();
    
    public void AddLaddersToFace(int faceIndex, bool[] ladderSlots, TierParameters tierParameters)
    {
        if (faces == null || faceIndex < 0 || faceIndex >= faces.Length)
            return;
        
        FaceController face = faces[faceIndex];

        for (int i = 0; i < ladderSlots.Length; i++)
        {
            if (!ladderSlots[i])
                continue;
            
            GameObject stairs = TaggedObjectPooler.Instance.GetPooledObject("Stairs");
            
            Transform slot = i < 3 ? face.GetUpperSlot(i) : face.GetLowerSlot(i % 3);
            
            stairs.transform.SetParent(slot, false);
            stairs.transform.localPosition  = Vector3.zero;
            stairs.transform.localRotation = Quaternion.identity;
            stairs.transform.localScale = Vector3.one;
            
            
            objects.Add(stairs);

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
            
            objects.Add(collectable);
        }
    }

    public void ClearObjects()
    {
        foreach (GameObject obj in objects)
        {
            obj.transform.SetParent(null); 
            TaggedObjectPooler.Instance.ReturnObject(obj, obj.tag);
        }
        
        objects.Clear();
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
