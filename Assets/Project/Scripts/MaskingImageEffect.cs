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
