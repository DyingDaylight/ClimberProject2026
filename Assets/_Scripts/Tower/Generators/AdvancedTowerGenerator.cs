
using System;
using System.Collections.Generic;
using _Scripts;
using _Scripts.Parameters;
using _Scripts.Poolers;
using UnityEngine;

public class SaveGameInfo
{
    public List<SegmentSaveInfo> segments;
    public float playerX;
    public float playerY;
}

public class SegmentSaveInfo
{
    public float posX;
    public float posY;
    public float posZ;
    public bool[] slots;
    
}

public class AdvancedTowerGenerator : BaseTowerGenerator
{
    [Header("Difficulty Management")]
    [SerializeField] private TowerParameterManager towerManager;

    public Action<string> OnDifficultyChanged;
    
    private int height = 0;
    
    // 0, 1, 2 - upper, 3, 4, 5 - lower slots
    private Dictionary<int, bool[]> upperLaddersOnFaces = new Dictionary<int, bool[]>();
    private bool isFirstSegment = true;

    private void Awake()
    {
        towerManager.ResetDifficulty();
        
        for (int i = 0; i < 4; i++)
        {
            upperLaddersOnFaces[i] = new bool[3];
        }
    }

    protected override void SpawnTier()
    {
        // spawn at nextSpawnY, then advance spawn position for the following tier
        GameObject newTier = TaggedObjectPooler.Instance.GetPooledObject("Tier");
        if (!newTier)
            return;
        
        newTier.transform.SetParent(transform, false);
        newTier.transform.position = new Vector3(0, nextSpawnY, 0);
        newTier.transform.rotation = Quaternion.identity;
        var currentParams = GetCurrentTierParameters();
        if (currentParams != null)
        {
            newTier.GetComponent<Renderer>().material = currentParams.tierMaterial;
        }
        
        tiers.Add(newTier);
        nextSpawnY += tierHeight;
        height++;

        TierPopulator populator = newTier.GetComponent<TierPopulator>();
        if (populator != null)
        {
            // TODO: fix bug with snapping to the ladders from different face
            for (int face = 0; face < 4; face++)
                populator.AddLaddersToFace(face, FindSlotsForLadders(face), 
                    towerManager.CurrentTierParameters);
        }

        if (towerManager != null && towerManager.IncrementTiers())
        {
            OnDifficultyChanged?.Invoke(towerManager.CurrentLevelName);
        }

        TowerTier tier = newTier.GetComponent<TowerTier>();
    }
    
    private LevelParameters GetCurrentLevelParameters()
    {
        // Try to get from parameters manager
        if (towerManager != null && towerManager.CurrentLevelParameters != null)
        {
            return towerManager.CurrentLevelParameters;
        }
        
        return null;
    }
    
    private TierParameters GetCurrentTierParameters()
    {
        // Try to get from parameters manager
        if (towerManager != null && towerManager.CurrentTierParameters != null)
        {
            return towerManager.CurrentTierParameters;
        }
        
        return null;
    }

    bool[] FindSlotsForLadders(int face)
    {
        bool[] slots = new bool[6];

        if (isFirstSegment)
        {
            for (int i = 0; i < upperLaddersOnFaces[face].Length; i++)
            {
                upperLaddersOnFaces[face][i] = GameRandom.Value() > 0.5f;
            }
            isFirstSegment = false;
        }
        
        for (int i = 0; i < 3; i++)
        {
            slots[i + 3] = upperLaddersOnFaces[face][i];
        }
        
        var tierParameters = GetCurrentTierParameters();
        int minLadders = tierParameters?.minLadder ?? 1;
        int maxLadders = tierParameters?.maxLadder ?? 3;
        int numLaddersToPlace = GameRandom.Range(minLadders, maxLadders + 1);

        for (int i = 0; i < 3; i++)
        {
            slots[i] = false;
            upperLaddersOnFaces[face][i] = false;
        }

        for (int i = 0; i < numLaddersToPlace; i++)
        {
            int randomIndex;
            do
            {
                randomIndex = GameRandom.Range(0, 3);
            } while (slots[randomIndex]);
            
            slots[randomIndex] = true;
            upperLaddersOnFaces[face][randomIndex] = true;
        }
        
        return slots;
    }
}
