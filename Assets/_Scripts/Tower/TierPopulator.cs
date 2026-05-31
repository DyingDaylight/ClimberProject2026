using System.Collections.Generic;
using _Scripts.Poolers;
using UnityEngine;

public class TierPopulator : MonoBehaviour
{
    [SerializeField] private FaceController[] faces;
    private List<GameObject> objects = new List<GameObject>();
    
    public void AddLaddersToFace(int faceIndex, bool[] ladderSlots)
    {
        if (faces == null || faceIndex < 0 || faceIndex >= faces.Length)
            return;
        
        FaceController face = faces[faceIndex];

        for (int i = 0; i < ladderSlots.Length; i++)
        {
            if (!ladderSlots[i])
                continue;
            
            GameObject stairs = TaggedObjectPooler.Instance.GetPooledObject("Stairs");
            
            Transform slot = i < 3 ? face.GetUpperSlot(i) : face.GetLowerSlot(i % 3);
            
            stairs.transform.SetParent(slot, false);
            stairs.transform.localPosition  = Vector3.zero;
            stairs.transform.localRotation = Quaternion.identity;
            stairs.transform.localScale = Vector3.one;
            
            objects.Add(stairs);
        }
    }

    public void ClearObjects()
    {
        foreach (GameObject obj in objects)
        {
            obj.transform.SetParent(null); 
            TaggedObjectPooler.Instance.ReturnObject(obj, obj.tag);
        }
        
        objects.Clear();
    }
}
