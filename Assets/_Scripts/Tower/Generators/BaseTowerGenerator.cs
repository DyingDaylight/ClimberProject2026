using System.Collections.Generic;
using _Scripts.Poolers;
using UnityEngine;

public abstract class BaseTowerGenerator : MonoBehaviour
{
    
    [SerializeField] private Camera mainCamera;
    [SerializeField] protected int tierHeight = 1; // Assumes the tier pivot is at its center
    
    protected List<GameObject> tiers = new List<GameObject>();
    protected float nextSpawnY = 0;
    
    private float drawHeight;

    private void Start()
    {
        float distance = Mathf.Abs(this.mainCamera.transform.position.z - transform.position.z);
        drawHeight = 2f * distance * Mathf.Tan(this.mainCamera.fieldOfView * 0.5f * Mathf.Deg2Rad);
    
        nextSpawnY = mainCamera.transform.position.y - drawHeight * 0.5f - tierHeight;
    }

    private void Update()
    {
        if (tiers.Count == 0)
        {
            InitializeTower();
            return;    
        }
    
        // if the top of the camera enters the current top tier, spawn the next one
        float upperEdge = this.mainCamera.transform.position.y + drawHeight * 0.5f;
        float lastTierLowerEdge = tiers[^1].transform.position.y - tierHeight / 2f;
        while (upperEdge >= lastTierLowerEdge)
        {
            SpawnTier();
            lastTierLowerEdge = tiers[^1].transform.position.y - tierHeight / 2f;
        }

        // if the bottom tier is completely below the visible area, remove it
        float lowerEdge = this.mainCamera.transform.position.y - drawHeight * 0.5f;
        while (tiers.Count > 0 && lowerEdge > tiers[0].transform.position.y + tierHeight / 2f)
        {
            RemoveBottomTier();
        }
    }

    private void InitializeTower()
    {
        int offsetTiers = 2;
        int initialTiersCount = (int) Mathf.CeilToInt(drawHeight / tierHeight) + offsetTiers;
    
        for (int i = 0; i < initialTiersCount; i++)
        {
            SpawnTier();
        }
    }

    protected abstract void SpawnTier();

    private void RemoveBottomTier()
    {
        if (tiers.Count == 1)
            return;
        
        GameObject bottomTier = tiers[0];
        TierPopulator tier = bottomTier.GetComponent<TierPopulator>();
        if (tier)
            tier.ClearObjects();
    
        tiers.RemoveAt(0);
        TaggedObjectPooler.Instance.ReturnObject(bottomTier, bottomTier.tag);
    }

    private void OnEnable()
    {
        WorldShiftController.OnWorldShift += HandleWorldShift;
    }

    private void OnDisable()
    {
        WorldShiftController.OnWorldShift -= HandleWorldShift;
    }

    private void HandleWorldShift(float deltaY)
    {
        if (tiers.Count == 0)
            return;
    
        foreach (GameObject tier in tiers)
        {
            tier.transform.position = new Vector3(tier.transform.position.x, 
                tier.transform.position.y + deltaY, tier.transform.position.z);
        }
        nextSpawnY = tiers[^1].transform.position.y + tierHeight;
    }
}