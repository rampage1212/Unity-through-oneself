using UnityEngine;

[RequireComponent(typeof(Camera))]
public class ReplaceTargetTexture : MonoBehaviour
{
    [SerializeField]
    RenderTextureGenerator.RenderTextureType replaceTextureWith;

    void Awake()
    {
        // Get camera
        Camera camera = GetComponent<Camera>();

        // by default, use the maske texture
        RenderTexture texture = RenderTextureGenerator.MaskingTexture;

        // Check which texture we actually want
        switch (replaceTextureWith)
        {
            case RenderTextureGenerator.RenderTextureType.InsideCharacter:
                texture = RenderTextureGenerator.InsideCharacterTexture;
                break;
        }

        // Update camera
        RenderTextureGenerator.ReplaceCameraTexture(camera, texture);
    }
}
