
using _Scripts.Poolers;
using UnityEngine;

public class AdvancedTowerGenerator : BaseTowerGenerator
{
    protected override void SpawnTier()
    {
        // spawn at nextSpawnY, then advance spawn position for the following tier
        GameObject newTier = TaggedObjectPooler.Instance.GetPooledObject("Tier");
        newTier.transform.SetParent(transform, false);
        newTier.transform.position = new Vector3(0, nextSpawnY, 0);
        newTier.transform.rotation = Quaternion.identity;
        
        for (int face = 0; face < 4; face++)
        {
            for (int slot = 0; slot < 3; slot++)
            {
                if (Random.value < GameManager.Instance.LevelParameters.stairsChance)
                {
                    GameObject stairs =  taggedObjectPooler.GetPooledObject("Stairs");

                    TierController tier = newTier.GetComponent<TierController>();
            
                    tier.PlaceObject(stairs, face, slot);
                }
            }
        }
        
    
        tiers.Add(newTier);
        nextSpawnY += tierHeight;
    }
}
