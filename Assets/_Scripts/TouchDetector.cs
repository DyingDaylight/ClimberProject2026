using UnityEngine;
using Touch = UnityEngine.InputSystem.EnhancedTouch.Touch;

public class TouchDetector : MonoBehaviour
{

    public float maxWaitingTime = 0.5f;
    public float minDistance = 100f;
    
    private Vector3 startSwipePosition;
    private int fingerId;
    private float swipeTimer;
    private bool isSwiping = false;
    
    
    // Update is called once per frame
    void Update()
    {
        if (Touch.activeFingers.Count > 0)
        {
            if (!isSwiping && Touch.activeFingers.Count == 1 && Touch.activeTouches[0].began)
            {
                startSwipePosition = Touch.activeTouches[0].screenPosition;
                fingerId = Touch.activeTouches[0].touchId;
                swipeTimer = 0;
                isSwiping = true;
            }

            if (isSwiping && Touch.activeFingers.Count == 1 && Touch.activeTouches[0].touchId == fingerId)
            {
                swipeTimer += Time.deltaTime;
                if (swipeTimer > maxWaitingTime)
                {
                    isSwiping = false;
                }
                else
                {
                    if (Touch.activeTouches[0].ended)
                    {
                        Vector3 touchPosition = Touch.activeTouches[0].screenPosition;
                        float distance = Vector3.Distance(startSwipePosition, touchPosition);
                        if (distance >= minDistance)
                            Debug.Log($"Swiped by {distance}");
                    }
                }
            }
            
            
        }    
    }
}
