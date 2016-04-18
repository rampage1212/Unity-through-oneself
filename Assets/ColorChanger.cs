using UnityEngine;

public class ColorChanger : MonoBehaviour
{
    [Tooltip("The saturation and value for the random color. Hue will be ignored")]
    [SerializeField]
    Color colorRange = Color.red;
    [SerializeField]
    Renderer[] allRenderers;

    void Awake()
    {
        HSBColor newColor = HSBColor.FromColor(colorRange);
        newColor.Hue = Random.value;
        colorRange = newColor.ToColor();
        foreach (Renderer render in allRenderers)
        {
            if(render != null)
            {
                foreach(Material mat in render.materials)
                {
                    colorRange.a = mat.color.a;
                    mat.color = colorRange;
                }
            }
        }
    }

#if UNITY_EDITOR
    [ContextMenu("Populate allRenderers")]
    void PopulateAllRenderers()
    {
        allRenderers = GetComponentsInChildren<Renderer>();
    }
#endif
}
