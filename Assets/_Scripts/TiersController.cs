using System.Collections.Generic;
using UnityEngine;

public class TiersController : MonoBehaviour
{
    [SerializeField] GameObject tierPrefab;
    [SerializeField] Camera camera;
    [SerializeField] int tierHeight = 1; // Assumes the tier pivot is at its center
    
    private List<GameObject> Tiers = new List<GameObject>();
    private float nextSpawnY = 0;
    
    void Start()
    {
        nextSpawnY = camera.transform.position.y - camera.orthographicSize - tierHeight;
        int offsetTiers = 2;
        int initialTiersCount = (int) Mathf.Ceil(camera.orthographicSize * 2 / tierHeight) + offsetTiers;
        
        for (int i = 0; i < initialTiersCount; i++)
        {
            SpawnTier();
        }
    }

    void Update()
    {
        if (Tiers.Count == 0)
        {
            Debug.Log("TiersController: no tiers found");
            return;    
        }
        
        // if the top of the camera enters the current top tier, spawn the next one
        float upperEdge = camera.transform.position.y + camera.orthographicSize;
        float lastTierLowerEdge = Tiers[^1].transform.position.y - tierHeight / 2f;
        while (upperEdge >= lastTierLowerEdge)
        {
            SpawnTier();
            lastTierLowerEdge = Tiers[^1].transform.position.y - tierHeight / 2f;
        }

        // if the bottom tier is completely below the visible area, remove it
        float lowerEdge = camera.transform.position.y - camera.orthographicSize;
        float bottomTierUpperEdge = Tiers[0].transform.position.y + tierHeight / 2f;
        while (Tiers.Count > 0 && lowerEdge > bottomTierUpperEdge)
        {
            RemoveBottomTier();
            bottomTierUpperEdge = Tiers[0].transform.position.y + tierHeight / 2f;
        }
    }

    private void SpawnTier()
    {
        // spawn at nextSpawnY, then advance spawn position for the following tier
        GameObject newTier = Instantiate(tierPrefab, new Vector3(0, nextSpawnY, 0), Quaternion.identity, transform);
        Tiers.Add(newTier);
        nextSpawnY += tierHeight;
    }

    private void RemoveBottomTier()
    {
        GameObject bottomTier = Tiers[0];
        Tiers.RemoveAt(0);
        Destroy(bottomTier);
    }
}
