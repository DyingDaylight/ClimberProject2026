using System.Collections;
using _Scripts;
using UnityEngine;
using UnityEngine.InputSystem;

public class TierSwipeRotator : MonoBehaviour
{
    [SerializeField] private MovementController playerMovement;
    [SerializeField] private float minSwipeDistance = 80f;

    [SerializeField] private Collider playerBodyCollider;
    [SerializeField] private float tierHeight = 1f;
    [SerializeField] private Transform towerRoot;
    
    private Vector2 startTouchPosition;
    private bool isTouching;
    private bool isBusy;
    
    private void Update()
    {
        if (Touchscreen.current == null)
            return;

        var touch = Touchscreen.current.primaryTouch;

        if (touch.press.wasPressedThisFrame)
        {
            startTouchPosition = touch.position.ReadValue();
            isTouching = true;
            return;
        }

        if (!touch.press.wasReleasedThisFrame || !isTouching)
            return;
        
        Vector2 endTouchPosition = touch.position.ReadValue();
        Vector2 delta = endTouchPosition - startTouchPosition;

        isTouching = false;

        if (Mathf.Abs(delta.x) < minSwipeDistance)
            return;

        if (isBusy)
            return;
        
        int direction = delta.x > 0f ? -1 : 1;

        TowerTier tierToRotate = GetTierAbovePlayer();
        
        // TODO: tier to rotate is null!
        if (tierToRotate == null)
        {
            Debug.Log("tierToRotate.IsRotating " + tierToRotate.IsRotating);
        }

        if (tierToRotate.IsRotating)
            return;

        Debug.Log("swiped");
        StartCoroutine(RotateTierSafely(tierToRotate, direction));
    }

    private IEnumerator RotateTierSafely(TowerTier tier, int direction)
    {
        isBusy = true;
        
        Debug.Log("Rotation started");

        tier.RotateQuarter(direction);

        yield return new WaitUntil(() => !tier.IsRotating);

        isBusy = false;
        
        Debug.Log("Rotation ended");
    }
    
    private TowerTier GetTierAbovePlayer()
    {
        float playerHeadY = playerBodyCollider.bounds.max.y;

        TowerTier bestTier = null;
        float bestBottomY = float.MaxValue;

        for (int i = 0; i < towerRoot.childCount; i++)
        {
            TowerTier candidate = towerRoot.GetChild(i).GetComponent<TowerTier>();

            if (candidate == null)
                continue;

            float candidateBottomY = candidate.transform.position.y - tierHeight * 0.5f;

            if (candidateBottomY <= playerHeadY  + 0.01f)
                continue;

            if (candidateBottomY < bestBottomY)
            {
                bestBottomY = candidateBottomY;
                bestTier = candidate;
            }
        }

        Debug.Log($"Player position: {playerMovement.name}, y={playerMovement.transform.position.y}");
        Debug.Log($"Tier above: {bestTier?.name}, y={bestTier?.transform.position.y}");
        
        return bestTier;
    }
}
