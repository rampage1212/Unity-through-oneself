using UnityEngine;

public abstract class IFormation : MonoBehaviour
{
    public abstract void UpdateBreadcrumbs(MoveToTargetCharacterControl movingObject);
}
