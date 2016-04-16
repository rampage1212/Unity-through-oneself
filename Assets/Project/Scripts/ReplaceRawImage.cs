using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(RawImage))]
public class ReplaceRawImage : MonoBehaviour
{
    [SerializeField]
    RenderTextureGenerator.RenderTextureType textureType;
    [SerializeField]
    Graphic[] allGraphicsToEnableOnStart;
    [SerializeField]
    Mask[] allMasksToEnableOnStart;

    // Use this for initialization
    void Awake ()
    {
        RawImage image = GetComponent<RawImage>();
        switch(textureType)
        {
            case RenderTextureGenerator.RenderTextureType.InsideCharacter:
                image.texture = RenderTextureGenerator.InsideCharacterTexture;
                break;
            default:
                image.texture = RenderTextureGenerator.MaskingTexture;
                break;
        }

        foreach(Graphic i in allGraphicsToEnableOnStart)
        {
            i.enabled = true;
        }
        foreach(Mask m in allMasksToEnableOnStart)
        {
            m.enabled = true;
        }
    }

    [ContextMenu("Fill allGraphicsToEnableOnStart")]
    void FillOutGraphics ()
    {
        allGraphicsToEnableOnStart = GetComponents<Graphic>();
        allMasksToEnableOnStart = GetComponents<Mask>();
    }
}
