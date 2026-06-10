using UnityEngine;
using UnityEngine.UI;

namespace _Scripts
{
    public class CameraCaptureToTexture : MonoBehaviour
    {
        public Camera captureCamera;
        public Texture2D captureTexture;
        public int textureWidth = 1920;
        public int textureHeight = 1080;

        public void Capture()
        {
            RenderTexture renderTexture = new RenderTexture(textureWidth, textureHeight, 24);
            captureCamera.targetTexture = renderTexture;
            
            captureTexture = new Texture2D(textureWidth, textureHeight, TextureFormat.RGB24, false);
            
            captureCamera.Render();
            
            RenderTexture.active = renderTexture;
            
            captureTexture.ReadPixels(new Rect(0, 0, renderTexture.width, renderTexture.height), 0, 0);
            captureTexture.Apply();
            
            RenderTexture.active = null;
            captureCamera.targetTexture = null;
            
            renderTexture.Release();
        }
    }
}