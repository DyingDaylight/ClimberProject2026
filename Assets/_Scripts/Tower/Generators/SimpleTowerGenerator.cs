using _Scripts.Poolers;
using UnityEngine;

public class SimpleTowerGenerator : BaseTowerGenerator
{
    protected override void SpawnTier()
    {
        // spawn at nextSpawnY, then advance spawn position for the following tier
        GameObject newTier = TaggedObjectPooler.Instance.GetPooledObject("Tier");
        newTier.transform.SetParent(transform, false);
        newTier.transform.position = new Vector3(0, nextSpawnY, 0);
        newTier.transform.rotation = Quaternion.identity;
        
        GameObject stairs =  taggedObjectPooler.GetPooledObject("Stairs");
        Transform slot = newTier.transform.Find("FaceBack/Zone_0");
    
        stairs.transform.SetParent(slot, false);
        stairs.transform.localPosition  = Vector3.zero;
        stairs.transform.localRotation = Quaternion.identity;
        stairs.transform.localScale = Vector3.one;
    
        tiers.Add(newTier);
        nextSpawnY += tierHeight;
    }
}