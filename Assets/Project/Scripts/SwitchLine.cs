using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
[ExecuteInEditMode]
public class SwitchLine : MonoBehaviour
{
    [SerializeField]
    Texture inactiveTexture;
    [SerializeField]
    Texture activeTexture;
#if UNITY_EDITOR
    [SerializeField]
    Transform[] allPoints = new Transform[10];
#endif

    LineRenderer rendererCache = null;
    bool isActive = false;

    LineRenderer CachedRenderer
    {
        get
        {
            if(rendererCache == null)
            {
                rendererCache = GetComponent<LineRenderer>();
            }
            return rendererCache;
        }
    }

    public bool IsOn
    {
        get
        {
            return isActive;
        }
        set
        {
            if(isActive != value)
            {
                isActive = value;
                if(isActive == true)
                {
                    CachedRenderer.material.mainTexture = activeTexture;
                }
                else
                {
                    CachedRenderer.material.mainTexture = inactiveTexture;
                }
            }
        }
    }

#if UNITY_EDITOR
    void Update ()
    {
        for(int i = 0; i < allPoints.Length; ++i)
        {
            if (allPoints[i] != null)
            {
                CachedRenderer.SetPosition(i, allPoints[i].localPosition);
            }
        }
    }
#endif
}
