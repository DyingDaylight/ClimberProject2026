using System;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    private Transform playerTransform;
    
    void Start()
    {
        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
        transform.position = new Vector3(transform.position.x, 
            playerTransform.position.y, 
            transform.position.z);
    }

    private void LateUpdate()
    {
        transform.position = new Vector3(transform.position.x, 
                                         playerTransform.position.y, 
                                         transform.position.z);
    }
}
