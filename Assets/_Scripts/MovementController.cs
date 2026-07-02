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

    protected Vector2 MovementInput = Vector2.zero;
    protected MovementMode movementMode = MovementMode.Platform;

    private Collider currentPlatform;
    protected HashSet<Collider> ladders = new HashSet<Collider>();
    private float laddersX;
    
    //private InputSystem_Actions inputActions;
    
    private float halfHeight = -1;
    
    private bool HasPlatform => currentPlatform != null;
    
    
    private void Awake()
    {
        //InitializePlayerInput();
    }

    protected virtual void Update()
    {
        ApplyMovement();
    }
    
    private void FixedUpdate()
    {
        // Count hight of the player and save it for later use in updates
        if (halfHeight < 0)
            halfHeight = transform.position.y - footCollider.bounds.min.y;
    }

    /*
    private void OnDestroy()
    {
        inputActions?.Dispose();
    }

    private void InitializePlayerInput()
    {
        inputActions = new InputSystem_Actions();
        inputActions.Player.Move.performed += OnMoveInput;
        inputActions.Player.Move.canceled += OnMoveInput;
        inputActions.Enable();
    }
    

    private void OnMoveInput(InputAction.CallbackContext context)
    {
        MovementInput = context.ReadValue<Vector2>();
    }
    */
    
    public void SetMovementInput(Vector2 input)
    {
        MovementInput = Vector2.ClampMagnitude(input, 1f);
    }
    
    private void ApplyMovement()
    {
        float horizontal = Mathf.Clamp(MovementInput.x, -1, 1);
        float vertical = Mathf.Clamp(MovementInput.y, -1, 1);

        Debug.Log("horizontal " + horizontal + " vertical " + vertical);
        
        Vector3 position = transform.position;

        Debug.Log("movementMode " + movementMode);
        switch (movementMode)
        {
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
        
        if (Mathf.Abs(vertical) > platformSnapOffset)
        {
            if (ladders.Count > 0)
            {
                movementMode = MovementMode.Ladder;
            }
        }

        return position;
    }
    
    private Vector3 MoveOnLadder(Vector3 position, float horizontal, float vertical)
    {
        if (Mathf.Abs(horizontal) > platformSnapOffset && currentPlatform)
        {
            movementMode = MovementMode.Platform;

            position.x += horizontal * horizontalSpeed * Time.deltaTime;
            position = SnapToPlatformY(position);
            position = ClampToPlatformX(position);
            
            return position;
        }
        
        if (ladders.Count == 0)
            return position;
        
        position.x = laddersX;
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
            Debug.Log("Platform entered " + other.name);
            currentPlatform = other;
        }
        
        if (colliderType == PlayerColliderType.Body && other.CompareTag("Stairs"))
        {
            Debug.Log("Ladder entered " + other.name);
            ladders.Add(other);
            // TODO: work on it
            laddersX = other.transform.position.x;
        }
    }
    
    public void OnChildTriggerExit(PlayerColliderType colliderType, Collider other)
    {
        if (colliderType == PlayerColliderType.Feet && other.CompareTag("Platform"))
        {
            Debug.Log("Platform exited " + other.name);
            currentPlatform = null;
        }
        
        if (colliderType == PlayerColliderType.Body && other.CompareTag("Stairs"))
        {
            Debug.Log("Ladder exited " + other.name);
            ladders.Remove(other);
        }
    }
}
