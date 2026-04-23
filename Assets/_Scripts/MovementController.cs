using System;
using UnityEngine;

public class MovementController : MonoBehaviour
{
    public void MoveUp() 
    {
        transform.position += Vector3.up;   
    }
}
