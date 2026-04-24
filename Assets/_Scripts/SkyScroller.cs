using UnityEngine;

public class SkyScroller : MonoBehaviour
{
    [SerializeField] Transform cameraTransform;
    [SerializeField] float parallaxFactor = 0.1f;

    private float currentOffestY = 0;
    private float lastCameraPositionY = 0;
    private Material material;
    private int baseMap = Shader.PropertyToID("_BaseMap");
    
    void Start()
    {
        material = GetComponent<MeshRenderer>().sharedMaterial;
        lastCameraPositionY = cameraTransform.position.y;
    }
    
    void LateUpdate()
    {
        float cameraPositionY = cameraTransform.position.y;
        float deltaY = cameraPositionY - lastCameraPositionY;
        
        currentOffestY += deltaY * parallaxFactor;
        currentOffestY = Mathf.Repeat(currentOffestY, 1f);
        
        material.SetTextureOffset(baseMap, new Vector2(0, currentOffestY));
        
        transform.position = new Vector3(
            transform.position.x,
            cameraTransform.position.y,
            transform.position.z
        );
        
        lastCameraPositionY = cameraPositionY;
    }
    
    private void OnEnable()
    {
        WorldShiftController.OnWorldShift += HandleWorldShift;
    }

    private void OnDisable()
    {
        WorldShiftController.OnWorldShift -= HandleWorldShift;
    }

    private void HandleWorldShift(float deltaY)
    {
        transform.position += Vector3.up * deltaY;
        lastCameraPositionY += deltaY;
    }
}
