using _Scripts;
using UnityEngine;

public class PlayerTriggerDetector : MonoBehaviour
{
    [SerializeField] private PlayerColliderType colliderType;
    [SerializeField] private MovementController movementController;

    private void OnTriggerEnter(Collider other)
    {
        movementController.OnChildTriggerEnter(colliderType, other);
    }

    private void OnTriggerExit(Collider other)
    {
        movementController.OnChildTriggerExit(colliderType, other);
    }
}
