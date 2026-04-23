using System;
using UnityEngine;

public class WorldShiftController : MonoBehaviour
{
    public static event Action<float> OnWorldShift;
    
    [SerializeField] private float maxPlayerDistance = 1000;
    [SerializeField] private float shiftTargetY = 0f;
    
    private Transform playerTransform;

    void Start()
    {
        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
    }
    
    void Update()
    {
        if (playerTransform.position.y >= maxPlayerDistance)
        {
            float deltaY = shiftTargetY - playerTransform.position.y;
            OnWorldShift?.Invoke(deltaY);
        }
    }
}
