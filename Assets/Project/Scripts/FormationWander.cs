using UnityEngine;
using System.Collections.Generic;

public class FormationWander : IFormation
{
    public const float PercentOfNpcsInWebGl = 0.5f;

    [SerializeField]
    Vector2 dimensions;
    [SerializeField]
    List<NonPlayableCharacter> allNpcs;

    Dictionary<MoveToTargetCharacterControl, Transform> allControllers = new Dictionary<MoveToTargetCharacterControl, Transform>();
    int updateIndex;

    [ContextMenu("Populate allNpcs")]
    void PopulateNpcs()
    {
        allNpcs.Clear();
        allNpcs.AddRange(GetComponentsInChildren<NonPlayableCharacter>());
    }

    void Start()
    {
        // Check if WebGL build
#if UNITY_WEBGL
        // Shuffle the NPC list
        OmiyaGames.Utility.ShuffleList<NonPlayableCharacter>(allNpcs);

        // Check how many NPCs we want to delete
        int reduceNpcsTo = Mathf.RoundToInt(allNpcs.Count * PercentOfNpcsInWebGl) + 1;

        // Go through the list from the back (more efficient than removing elements from the fron)
        for (updateIndex = (allNpcs.Count - 1); updateIndex > (allNpcs.Count - reduceNpcsTo); --updateIndex)
        {
            // Destroy the last NPC in the list
            if (allNpcs[updateIndex] != null)
            {
                Destroy(allNpcs[updateIndex].gameObject);
            }

            // Pop this entry off the list
            allNpcs.RemoveAt(updateIndex);
        }
#endif

        // Make a transform for each NPC to walk towards
        GameObject newObject = null;
        allControllers.Clear();
        for (updateIndex = 0; updateIndex < allNpcs.Count; ++updateIndex)
        {
            if (allNpcs[updateIndex] != null)
            {
                newObject = new GameObject(allNpcs[updateIndex].name + " Target");
                newObject.transform.SetParent(transform);
                allControllers.Add(allNpcs[updateIndex].CachedAi, newObject.transform);

                UpdateBreadcrumbs(allNpcs[updateIndex].CachedAi);
            }
        }
    }

    void Update()
    {
        for (updateIndex = 0; updateIndex < allNpcs.Count; ++updateIndex)
        {
            if (allNpcs[updateIndex] != null)
            {
                allNpcs[updateIndex].UpdateTarget(this);
            }
        }
    }

    void OnDrawGizmos()
    {
        Vector3 area = new Vector3(dimensions.x, 1, dimensions.y);
        Gizmos.DrawWireCube(transform.position, area);
    }

    public float RandomX
    {
        get
        {
            return Random.Range(-dimensions.x / 2f, dimensions.x / 2f);
        }
    }

    public float RandomZ
    {
        get
        {
            return Random.Range(-dimensions.y / 2f, dimensions.y / 2f);
        }
    }

    public override void UpdateBreadcrumbs(MoveToTargetCharacterControl movingObject)
    {
        Transform target;
        if(allControllers.TryGetValue(movingObject, out target) == true)
        {
            target.localPosition = new Vector3(RandomX, 0, RandomZ);
            movingObject.AddBreadcrumb(target);
        }
    }
}
