using UnityEngine;

public class LadderSegmentInfo : MonoBehaviour
{

    public enum LadderSegmentType
    {
        Upper,
        Lower
    }
    
    public LadderSegmentType Type { get; private set; }
    public int FaceIndex { get; private set; }
    public int SlotIndex { get; private set; }

    public void Initialize(LadderSegmentType type, int faceIndex, int slotIndex)
    {
        Type = type;
        FaceIndex = faceIndex;
        SlotIndex = slotIndex;
    }
}
