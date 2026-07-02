using UnityEngine;
using UnityEngine.InputSystem;

namespace _Scripts
{
    public class PlayerMovementInput : MonoBehaviour
    {
        [SerializeField] private MovementController movementController;

        private InputSystem_Actions inputActions;

        private void OnDestroy()
        {
            inputActions?.Dispose();
        }
        
        private void Awake()
        {
            inputActions = new InputSystem_Actions();

            inputActions.Player.Move.performed += OnMoveInput;
            inputActions.Player.Move.canceled += OnMoveInput;

            inputActions.Enable();
        }


        private void OnMoveInput(InputAction.CallbackContext context)
        {
            Vector2 input = context.ReadValue<Vector2>();
            movementController.SetMovementInput(input);
        }
    }
}