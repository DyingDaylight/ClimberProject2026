using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerControl : MonoBehaviour
{
    [Header("Movement")] [SerializeField] private float horizontalSpeed = 5f;
    [SerializeField] private float verticalSpeed = 4f;

    
    [Header("Boundary Checking")] [SerializeField]
    private float boundaryBuffer = 0.1f; // Small buffer to prevent edge clipping

    public Vector2 MovementInput { get; set; } = Vector2.zero;

    // Current state
    private MovementMode movementMode = MovementMode.Platform;

    private bool isOnPlatform = true;
    private bool isOnLadder = false;


    private Vector3 ladderCenter;

    // Platform boundary tracking
    private Collider currentPlatform;
    private Bounds platformBounds;

    
    //private InputSystemActions inputActions;
    

    public enum MovementMode
    {
        Platform,
        Ladder
    }

    private Collider playerCollider;


    private void Awake()
    {
        playerCollider = GetComponent<Collider>();
        movementMode = MovementMode.Platform;
        InitializePlayerInput();

    }

    private void OnDestroy()
    {
        //inputActions?.Dispose();
    }

    private void InitializePlayerInput()
    {
        /*inputActions = new InputSystemActions();
        inputActions.Player.Move.performed += OnMoveInput;
        inputActions.Player.Move.canceled += OnMoveInput;
        inputActions.Enable();*/
    }

    private void OnMoveInput(InputAction.CallbackContext context)
    {
        MovementInput = context.ReadValue<Vector2>();
    }

    private void Update()
    {
        ApplyMovement();
    }




    private void ApplyMovement()
    {
        Vector3 move = Vector3.zero;
        float horizontalInput = Mathf.Clamp(MovementInput.x, -1f, 1f);
        float verticalInput = Mathf.Clamp(MovementInput.y, -1f, 1f);

        if (movementMode == MovementMode.Platform)
        {

            move.x = horizontalInput * horizontalSpeed;

            if (isOnLadder && Mathf.Abs(verticalInput) > 0.01f)
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

            if (Mathf.Abs(horizontalInput) > 0.01f && isOnPlatform)
            {
                ExitLadderMode();
                move.x = horizontalInput * horizontalSpeed;
            }
            else
            {
                Vector3 position = transform.position;
                position.x = ladderCenter.x;
                transform.position = position;
                move.y = verticalInput * verticalSpeed;
            }
        }

        // Apply movement with boundary checking
        Vector3 newPosition = transform.position + move * Time.deltaTime;

        // Clamp to platform boundaries if on platform
        if (currentPlatform != null && movementMode == MovementMode.Platform)
        {
            newPosition = ClampToPlatformBounds(newPosition);
        }

        transform.position = newPosition;
    }


    private Vector3 ClampToPlatformBounds(Vector3 targetPosition)
    {
       
        if (playerCollider == null) return targetPosition;

        // Calculate the effective boundaries considering player size
        float playerHalfWidth = playerCollider.bounds.size.x * 0.5f;
        float leftBound = platformBounds.min.x + playerHalfWidth + boundaryBuffer;
        float rightBound = platformBounds.max.x - playerHalfWidth - boundaryBuffer;

        // Clamp horizontal position
        targetPosition.x = Mathf.Clamp(targetPosition.x, leftBound, rightBound);
        targetPosition.y =
            platformBounds.max.y + playerCollider.bounds.size.y; // Ensure player stays on top of platform

        return targetPosition;
    }




    private void EnterLadderMode()
    {
        isOnLadder = true;
        movementMode = MovementMode.Ladder;
    
        // Snap immediately to ladder
        Vector3 position = transform.position;

        position.x = ladderCenter.x;


        transform.position = position;
        Debug.Log("Entered ladder mode");
    }

    private void ExitLadderMode()
    {
        isOnLadder = false;
        Debug.Log("exit ladder mode");
        movementMode = MovementMode.Platform;
        // Snap immediately to platform
        Vector3 position = transform.position;

        position.y = platformBounds.max.y + playerCollider.bounds.size.y;
    }

    // =========================================================
    // TRIGGER DETECTOR CALLBACKS
    // =========================================================

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Ladder"))
        {
            ladderCenter = other.bounds.center;
            EnterLadderMode();
        }
        else if (other.gameObject.CompareTag("Platform"))
        {
            isOnPlatform = true;
            currentPlatform = other;
            platformBounds = other.bounds;
        }

    }

    void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Ladder"))
        {
            ExitLadderMode();
        }
        else if (other.gameObject.CompareTag("Platform"))
        {
            isOnPlatform = false;
            if (currentPlatform == other)
            {
                currentPlatform = null;
            }
        }
    }
}









