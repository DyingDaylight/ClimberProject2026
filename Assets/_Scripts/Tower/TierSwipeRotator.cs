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
        
        if (tierToRotate.IsRotating)
            return;

        StartCoroutine(RotateTierSafely(tierToRotate, direction));
    }

    private IEnumerator RotateTierSafely(TowerTier tier, int direction)
    {
        isBusy = true;
        
        tier.RotateQuarter(direction);

        yield return new WaitUntil(() => !tier.IsRotating);

        isBusy = false;
    }
    
    private TowerTier GetTierAbovePlayer()
    {
        float playerHeadY = playerBodyCollider.bounds.max.y - 0.05f;

        TowerTier bestTier = null;
        float bestBottomY = float.MaxValue;

        for (int i = 0; i < towerRoot.childCount; i++)
        {
            TowerTier candidate = towerRoot.GetChild(i).GetComponent<TowerTier>();

            if (candidate == null)
                continue;

            float candidateBottomY = candidate.transform.position.y - tierHeight * 0.5f;

            if (candidateBottomY <= playerHeadY)
                continue;

            if (candidateBottomY < bestBottomY)
            {
                bestBottomY = candidateBottomY;
                bestTier = candidate;
            }
        }

        return bestTier;
    }
}
