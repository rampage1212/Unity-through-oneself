using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(MoveToTargetCharacterControl))]
public class NonPlayableCharacter : MonoBehaviour
{
    [SerializeField]
    float standFor = 1f;
    [SerializeField]
    Vector2 resizeRange = new Vector2(0.75f, 1.25f);

    MoveToTargetCharacterControl aiCache = null;

    public MoveToTargetCharacterControl CachedAi
    {
        get
        {
            if(aiCache == null)
            {
                aiCache = GetComponent<MoveToTargetCharacterControl>();
            }
            return aiCache;
        }
    }

    void Awake()
    {
        transform.localScale = Vector3.one * Random.Range(resizeRange.x, resizeRange.y);
    }

    public void UpdateTarget(IFormation formation)
    {
        if(CachedAi.DurationNotFollowingTarget > standFor)
        {
            // set a new target
            formation.UpdateBreadcrumbs(CachedAi);
        }
    }
}
