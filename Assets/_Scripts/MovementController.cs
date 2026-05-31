using System;
using _Scripts;
using UnityEngine;
using UnityEngine.InputSystem;

public class MovementController : MonoBehaviour
{
    [Header("Movement Speed")]
    [SerializeField] float horizontalSpeed = 5f;
    [SerializeField] float verticalSpeed = 4f;

    [Header("Boundary Checking")] [SerializeField]
    private float boundaryBuffer = 0.01f;

    private Vector2 MovementInput { get; set; } = Vector2.zero;

    private MovementMode movementMode = MovementMode.Platform;

    private bool isOnPlatform = true;
    private bool isOnLadder = false;

    private Vector3 ladderCenter;

    private Collider playerCollider;
    private Collider currentPlatform;
    private Bounds platformBounds;
    
    private InputSystem_Actions inputActions;

    private void Awake()
    {
        playerCollider = GetComponent<Collider>();
        movementMode = MovementMode.Platform;
        InitializePlayerInput();
    }

    private void Update()
    {
        ApplyMovement();
    }

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

    private void OnMoveInput(InputAction.CallbackContext ctx)
    {
        MovementInput = ctx.ReadValue<Vector2>();
    }

    private void ApplyMovement()
    {
        Vector3 movementVector = Vector3.zero;
        float horizontal = Mathf.Clamp(MovementInput.x, -1, 1);
        float vertical = Mathf.Clamp(MovementInput.y, -1, 1);

        if (movementMode == MovementMode.Platform)
        {
            movementVector.x = horizontal * horizontalSpeed;

            if (isOnLadder && Mathf.Abs(vertical) > 0.01f)
            {
                EnterLadderMode();
            }
        } 
        else if (movementMode == MovementMode.Ladder)
        {
            if (!isOnLadder)
            {
                ExitLadderMode();
                return;
            }

            if (Mathf.Abs(horizontal) > 0.01f && isOnPlatform)
            {
                ExitLadderMode();
                movementVector.x = horizontal *  horizontalSpeed;
            }
            else
            {
                Vector3 position = transform.position;
                position.x = ladderCenter.x;
                transform.position = position;
                movementVector.y = vertical * verticalSpeed;
            }
        }
        
        Vector3 newPosition = transform.position + movementVector * Time.deltaTime;

        if (currentPlatform != null && movementMode == MovementMode.Platform)
        {
            newPosition = ClampToPlatformBounds(newPosition);
        }
        
        transform.position = newPosition;
    }

    private Vector3 ClampToPlatformBounds(Vector3 position)
    {
        if (playerCollider == null) return position;
        
        float playerHalfWidth = playerCollider.bounds.size.x * 0.5f;
        float leftBound = platformBounds.min.x + playerHalfWidth;
        float rightBound = platformBounds.max.x - playerHalfWidth;
        
        position.x = Mathf.Clamp(position.x, leftBound, rightBound);
        position.y = platformBounds.min.y + playerCollider.bounds.size.y;
        
        return position;
    }

    private void EnterLadderMode()
    {
        isOnLadder = true;
        movementMode = MovementMode.Ladder;
        
        Vector3 position = transform.position;
        position.x = ladderCenter.x;
        transform.position = position;
        
        Debug.Log("Entered ladder mode");
    }

    private void ExitLadderMode()
    {
        isOnLadder = false;
        movementMode = MovementMode.Platform;
        
        Vector3 position = transform.position;
        position.y = platformBounds.max.y + playerCollider.bounds.size.y;
        transform.position = position;
        
        Debug.Log("Exited ladder mode");
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
    
    public void MoveUp() 
    {
        transform.position += Vector3.up;   
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Stairs"))
        {
            ladderCenter = other.bounds.center;
            EnterLadderMode();
        }
        else if (other.CompareTag("Platform"))
        {
            isOnPlatform = true;
            currentPlatform = other;
            platformBounds = other.bounds;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Stairs"))
        {
            ExitLadderMode();
        }
        else if (other.CompareTag("Platform"))
        {
            isOnPlatform = false;
            if (currentPlatform == other) currentPlatform = null;
        }
    }
}
