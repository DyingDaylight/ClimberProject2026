using UnityEngine;
using UnityEngine.UI;

public class WebCamBasic : MonoBehaviour
{
    
    public RawImage rawImage;
    private WebCamTexture webCamTexture;
    WebCamDevice[] devices;
    
    void Start()
    {
        devices = WebCamTexture.devices;
        if (devices.Length > 0)
        {
            WebCamDevice device = devices[0];
            webCamTexture = new WebCamTexture(device.name);
            rawImage.texture = webCamTexture;
            webCamTexture.Play();
        }
    }

    
}
