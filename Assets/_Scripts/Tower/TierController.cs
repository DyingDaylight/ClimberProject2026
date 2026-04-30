using System.Collections.Generic;
using _Scripts.Poolers;
using UnityEngine;

public class TierController : MonoBehaviour
{

    [SerializeField] private FaceController[] faces;

    private List<GameObject> objects = new List<GameObject>();

    public Transform GetSlot(int faceIndex, int slotIndex)
    {
        if (faceIndex < 0 || faceIndex >= faces.Length)
            return null;

        var face = faces[faceIndex];

        if (slotIndex < 0 || slotIndex >= face.SlotsCount)
            return null;

        return face.GetSlot(slotIndex);
    }

    public void PlaceObject(GameObject obj, int face, int slotIndex)
    {
        Transform slot = GetSlot(face, slotIndex);

        obj.transform.SetParent(slot, false);
        obj.transform.localPosition  = Vector3.zero;
        obj.transform.localRotation = Quaternion.identity;
        obj.transform.localScale = Vector3.one;

        objects.Add(obj);
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
