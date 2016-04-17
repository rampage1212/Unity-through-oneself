using UnityEngine;
using UnityStandardAssets.Characters.ThirdPerson;

[RequireComponent(typeof (ThirdPersonCharacter))]
public class MoveToTargetCharacterControl : MonoBehaviour
{
    public NavMeshAgent agent { get; private set; }             // the navmesh agent required for the path finding
    public ThirdPersonCharacter character { get; private set; } // the character we are controlling
    [SerializeField]
    Transform target;                                    // target to aim for
    [SerializeField]
    float maxMoveVelocity = 2f;
    [SerializeField]
    float acceleration = 2f;
    [SerializeField]
    float stopRange = 0.2f;

    private void Start()
    {
        // get the components on the object we need ( should not be null due to require component so no need to check )
        agent = GetComponentInChildren<NavMeshAgent>();
        character = GetComponent<ThirdPersonCharacter>();

	    agent.updateRotation = false;
	    agent.updatePosition = true;
    }


    private void Update()
    {
        Vector3 moveDirection = Vector3.zero;
        if (target != null)
        {
            moveDirection = target.position - transform.position;
            if(moveDirection.sqrMagnitude < (stopRange * stopRange))
            {
                moveDirection = Vector3.zero;
            }
            //else
            //{

            //}
        }

        character.Move(moveDirection, false, false);
    }


    public void SetTarget(Transform target)
    {
        this.target = target;
    }
}
