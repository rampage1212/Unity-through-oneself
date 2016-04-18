using UnityEngine;
using System.Collections.Generic;

public class FormationWander : IFormation
{
    [SerializeField]
    Vector2 dimensions;

    NonPlayableCharacter[] allNpcs = null;
    Dictionary<MoveToTargetCharacterControl, Transform> allControllers = new Dictionary<MoveToTargetCharacterControl, Transform>();
    int updateIndex;

    void Start()
    {
        allNpcs = GetComponentsInChildren<NonPlayableCharacter>();
        allControllers.Clear();

        GameObject newObject = null;
        foreach(NonPlayableCharacter npc in allNpcs)
        {
            newObject = new GameObject(npc.name + " Target");
            newObject.transform.SetParent(transform);
            allControllers.Add(npc.CachedAi, newObject.transform);

            UpdateBreadcrumbs(npc.CachedAi);
        }
    }

    void Update()
    {
        for (updateIndex = 0; updateIndex < allNpcs.Length; ++updateIndex)
        {
            allNpcs[updateIndex].UpdateTarget(this);
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
