using UnityEngine;

public class FaceController : MonoBehaviour
{
    [SerializeField] Transform[] lowerSlots;
    [SerializeField] Transform[] upperSlots;

    public int LowerSlotsCount => lowerSlots.Length;
    public int UpperSlotsCount => upperSlots.Length;

    public Transform GetUpperSlot(int index)
    {
        if (index < 0 || index >= upperSlots.Length)
        {
            Debug.LogError($"Face index {index} out of range");
            return null;
        }
        
        return upperSlots[index];
    }
    
    public Transform GetLowerSlot(int index)
    {
        if (index < 0 || index >= lowerSlots.Length)
        {
            Debug.LogError($"Face index {index} out of range");
            return null;
        }
        
        return lowerSlots[index];
    }
}

