using UnityEngine;

public class RenderTextureGenerator
{
    public enum RenderTextureType
    {
        Mask,
        InsideCharacter
    }
    static RenderTexture maskingTexture = null;
    static RenderTexture insideCharacterTexture = null;

    public static RenderTexture MaskingTexture
    {
        get
        {
            if(maskingTexture == null)
            {
                maskingTexture = new RenderTexture(Screen.width, Screen.height, 16);
            }
            return maskingTexture;
        }
    }

    public static RenderTexture InsideCharacterTexture
    {
        get
        {
            if (insideCharacterTexture == null)
            {
                insideCharacterTexture = new RenderTexture(Screen.width, Screen.height, 16);
            }
            return insideCharacterTexture;
        }
    }

    public static void ReplaceCameraTexture(Camera renderCamera, RenderTexture renderTexture)
    {
        if (renderCamera.targetTexture != null)
        {
            renderCamera.targetTexture.Release();
        }
        renderCamera.targetTexture = renderTexture;
    }
}
