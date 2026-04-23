using System;
using UnityEngine;

public class MovementController : MonoBehaviour
{
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
}
