using UnityEngine;

public class FaceController : MonoBehaviour
{
    [SerializeField] Transform[] slots;

    public int SlotsCount => slots.Length;

    public Transform GetSlot(int index)
    {
        if (index < 0 || index >= slots.Length)
        {
            Debug.LogError("Face index out of range");
            return null;
        }
        
        return slots[index];
    }
}

