using System;
using System.Collections.Generic;
using System.Linq;
using _Scripts;
using UnityEngine;
using UnityEngine.InputSystem;

public class MovementController : MonoBehaviour
{
    [Header("Movement Speed")]
    [SerializeField] private float horizontalSpeed = 1f;
    [SerializeField] private float verticalSpeed = 1f;

    [Header("Colliders")]
    [SerializeField] private Collider footCollider;
    [SerializeField] private float platformSnapOffset = 0.01f;
    
    [Header("Ladder Alignment")]
    [SerializeField] private float ladderAlignSpeed = 3f;
    [SerializeField] private float ladderAlignThreshold = 0.03f;

    protected Vector2 MovementInput = Vector2.zero;
    protected MovementMode movementMode = MovementMode.Platform;

    private Collider currentPlatform;
    
    protected HashSet<Collider> ladders = new HashSet<Collider>();
    protected Collider currentLadderCollider;
    protected LadderSegmentInfo currentLadderInfo;
    protected LadderSegmentInfo.LadderSegmentType? lastExitedLadderType;
    
    private float halfHeight = -1;

    public TowerTier CurrentTier { get; private set; }

    public bool IsOnLadder => currentLadderCollider != null;
    public LadderSegmentInfo CurrentLadderInfo => currentLadderInfo;
    public LadderSegmentInfo.LadderSegmentType? LastExitedLadderType => lastExitedLadderType;
    
    private bool HasPlatform => currentPlatform != null;


    protected virtual void Update()
    {
        ApplyMovement();
    }
    
    private void FixedUpdate()
    {
        // Count height of the player and save it for later use in updates
        if (halfHeight < 0 && footCollider != null)
            halfHeight = transform.position.y - footCollider.bounds.min.y;
    }
    
    public void SetMovementInput(Vector2 input)
    {
        MovementInput = Vector2.ClampMagnitude(input, 1f);
    }

    public void SetMovementMode(MovementMode mode)
    {
        movementMode = mode;
    }
    
    public void ClearLastExitedLadder()
    {
        lastExitedLadderType = null;
    }
    
    private void ApplyMovement()
    {
        float horizontal = Mathf.Clamp(MovementInput.x, -1, 1);
        float vertical = Mathf.Clamp(MovementInput.y, -1, 1);

        Vector3 position = transform.position;

        switch (movementMode)
        {
            case MovementMode.None:
                MovementInput = Vector2.zero;
                return;
            
            case MovementMode.Platform:
                position = MoveOnPlatform(position, horizontal, vertical);
                break;

            case MovementMode.Ladder:
                position = MoveOnLadder(position, horizontal, vertical);
                break;
        }

        transform.position = position;
    }

    private Vector3 MoveOnPlatform(Vector3 position, float horizontal, float vertical)
    {
        if (currentPlatform == null)
        {
            Debug.Log("No current platform, cannot move");
            return position;
        }

        position.x += horizontal * horizontalSpeed * Time.deltaTime;
        position = ClampToPlatformX(position);
        position = SnapToPlatformY(position);

        return position;
    }
    
    private Vector3 MoveOnLadder(Vector3 position, float horizontal, float vertical)
    {
        if (currentLadderCollider == null)
            return position;
        
        float targetX = currentLadderCollider.transform.position.x;
        position.x = Mathf.MoveTowards(position.x, targetX, ladderAlignSpeed * Time.deltaTime);
        bool alignedToLadder = Mathf.Abs(position.x - targetX) <= ladderAlignThreshold;
        
        if (!alignedToLadder)
            return position;
        
        position.x = targetX;
        position.y += vertical * verticalSpeed * Time.deltaTime;
        
        return position;
    }

    private Vector3 SnapToPlatformY(Vector3 position)
    {
        if (!HasPlatform || footCollider == null || halfHeight < 0)
            return position;

        position.y = currentPlatform.bounds.max.y + halfHeight - platformSnapOffset;
        return position;
    }

    private Vector3 ClampToPlatformX(Vector3 position)
    {
        if (!HasPlatform || footCollider == null) 
            return position;
        
        Bounds platformBounds = currentPlatform.bounds;
        
        float playerHalfWidth = footCollider.bounds.size.x * 0.5f;
        float leftBound = platformBounds.min.x + playerHalfWidth;
        float rightBound = platformBounds.max.x - playerHalfWidth;
        
        position.x = Mathf.Clamp(position.x, leftBound, rightBound);
        return position;
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
        transform.position = new Vector3(transform.position.x, 
            transform.position.y + deltaY, transform.position.z);
    }

    public void OnChildTriggerEnter(PlayerColliderType colliderType, Collider other)
    {
        if (colliderType == PlayerColliderType.Feet && other.CompareTag("Platform"))
        {
            currentPlatform = other;
            CurrentTier = other.GetComponentInParent<TowerTier>();
        }
        
        if (colliderType == PlayerColliderType.Body && other.CompareTag("Stairs"))
        {
            ladders.Add(other);
            
            currentLadderCollider = other;
            currentLadderInfo = other.GetComponentInParent<LadderSegmentInfo>();
            
            lastExitedLadderType = null;
        }
    }
    
    public void OnChildTriggerExit(PlayerColliderType colliderType, Collider other)
    {
        if (colliderType == PlayerColliderType.Feet && other.CompareTag("Platform"))
        {
            if (currentPlatform == other)
                currentPlatform = null;
        }
        
        if (colliderType == PlayerColliderType.Body && other.CompareTag("Stairs"))
        {
            LadderSegmentInfo exitedInfo = other.GetComponentInParent<LadderSegmentInfo>();

            if (exitedInfo != null)
                lastExitedLadderType = exitedInfo.Type;

            ladders.Remove(other);

            if (currentLadderCollider == other)
            {
                currentLadderCollider = null;
                currentLadderInfo = null;
            }
        }
    }
}
