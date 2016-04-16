using UnityEngine;

[ExecuteInEditMode]
public class MaskingImageEffect : MonoBehaviour
{
    [SerializeField]
    Texture maskTexture;
    Material material;

    void Start()
    {
        material = new Material(Shader.Find("Hidden/MaskShader"));
        material.SetTexture("Mask", maskTexture);
    }

    //Postprocess the image
    void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        Graphics.Blit(source, destination, material);
    }
}

//using UnityEngine;
//using System.Collections;

//[ExecuteInEditMode]
//public class CRTEffect : MonoBehaviour
//{
//    public Material material;

//    // Postprocess the image
//    void OnRenderImage(RenderTexture source, RenderTexture destination)
//    {
//        Graphics.Blit(source, destination, material);
//    }
//}
