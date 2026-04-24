using System.Collections.Generic;
using UnityEngine;

public class TiersController : MonoBehaviour
{
    [SerializeField] GameObject tierPrefab;
    [SerializeField] Camera camera;
    [SerializeField] int tierHeight = 1; // Assumes the tier pivot is at its center
    
    private List<GameObject> tiers = new List<GameObject>();
    private float nextSpawnY = 0;
    private float visibleHeight;
    
    void Start()
    {
        
        float distance = Mathf.Abs(camera.transform.position.z - transform.position.z);
        visibleHeight = 2f * distance * Mathf.Tan(camera.fieldOfView * 0.5f * Mathf.Deg2Rad);
        
        nextSpawnY = camera.transform.position.y - visibleHeight * 0.5f - tierHeight;
        int offsetTiers = 2;
        int initialTiersCount = (int) Mathf.CeilToInt(visibleHeight / tierHeight) + offsetTiers;
        
        for (int i = 0; i < initialTiersCount; i++)
        {
            SpawnTier();
        }
    }

    void Update()
    {
        if (tiers.Count == 0)
        {
            Debug.Log("TiersController: no tiers found");
            return;    
        }
        
        // if the top of the camera enters the current top tier, spawn the next one
        float upperEdge = camera.transform.position.y + visibleHeight * 0.5f;
        float lastTierLowerEdge = tiers[^1].transform.position.y - tierHeight / 2f;
        while (upperEdge >= lastTierLowerEdge)
        {
            SpawnTier();
            lastTierLowerEdge = tiers[^1].transform.position.y - tierHeight / 2f;
        }

        // if the bottom tier is completely below the visible area, remove it
        float lowerEdge = camera.transform.position.y - visibleHeight * 0.5f;
        while (tiers.Count > 0 && lowerEdge > tiers[0].transform.position.y + tierHeight / 2f)
        {
            RemoveBottomTier();
        }
    }

    private void SpawnTier()
    {
        // spawn at nextSpawnY, then advance spawn position for the following tier
        GameObject newTier = Instantiate(tierPrefab, new Vector3(0, nextSpawnY, 0), Quaternion.identity, transform);
        tiers.Add(newTier);
        nextSpawnY += tierHeight;
    }

    private void RemoveBottomTier()
    {
        GameObject bottomTier = tiers[0];
        tiers.RemoveAt(0);
        Destroy(bottomTier);
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
