using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowerTier : MonoBehaviour
{
    [SerializeField] private float rotationDuration = 0.25f;

    public bool IsRotating { get; private set; }
    
    public void RotateQuarter(int direction)
    {
        if (IsRotating)
            return;

        StartCoroutine(RotateCoroutine(direction));
    }
    
    private IEnumerator RotateCoroutine(int direction)
    {
        IsRotating = true;

        Quaternion startRotation = transform.rotation;
        Quaternion targetRotation =
            startRotation * Quaternion.Euler(0f, 90f * direction, 0f);

        float timer = 0f;

        while (timer < rotationDuration)
        {
            timer += Time.deltaTime;
            float t = timer / rotationDuration;

            transform.rotation = Quaternion.Slerp(
                startRotation,
                targetRotation,
                t
            );

            yield return null;
        }

        transform.rotation = targetRotation;
        IsRotating = false;
    }
}
