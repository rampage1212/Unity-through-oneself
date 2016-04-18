using UnityEngine;
using UnityStandardAssets.Characters.ThirdPerson;
using System.Collections.Generic;

[RequireComponent(typeof (ThirdPersonCharacter))]
public class MoveToTargetCharacterControl : MonoBehaviour
{
    public ThirdPersonCharacter character { get; private set; } // the character we are controlling
    [SerializeField]
    float maxMoveVelocity = 2f;
    [SerializeField]
    float stopRange = 0.2f;
    [SerializeField]
    bool disregardYAxis = true;

    float lastTimeFollowingTarget = -1f;
    readonly Queue<Transform> breadcrumbs = new Queue<Transform>();

    public float DurationNotFollowingTarget
    {
        get
        {
            return Time.time - lastTimeFollowingTarget;
        }
    }

    public void AddBreadcrumb(Transform newBreadCrumb)
    {
        if(newBreadCrumb != null)
        {
            breadcrumbs.Enqueue(newBreadCrumb);
        }
    }

    public void ClearBreadcrumbs()
    {
        breadcrumbs.Clear();
    }

    private void Start()
    {
        character = GetComponent<ThirdPersonCharacter>();
        lastTimeFollowingTarget = Time.time;
    }

    private void Update()
    {
        Vector3 moveDirection = Vector3.zero;
        if (breadcrumbs.Count > 0)
        {
            moveDirection = breadcrumbs.Peek().position - transform.position;
            if(disregardYAxis == true)
            {
                moveDirection.y = 0;
            }
            if (moveDirection.sqrMagnitude < (stopRange * stopRange))
            {
                // Zero movement
                moveDirection.x = 0;
                moveDirection.y = 0;
                moveDirection.z = 0;

                // Remove this breadcrumb
                breadcrumbs.Dequeue();
            }
            else
            {
                lastTimeFollowingTarget = Time.time;
                if (moveDirection.sqrMagnitude > (maxMoveVelocity * maxMoveVelocity))
                {
                    moveDirection.Normalize();
                    moveDirection *= maxMoveVelocity;
                }
            }
        }

        character.Move(moveDirection, false, false);
    }
}
