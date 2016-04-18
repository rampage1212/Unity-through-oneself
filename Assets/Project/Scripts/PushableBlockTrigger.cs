using UnityEngine;

[RequireComponent(typeof(Collider))]
public class PushableBlockTrigger : MonoBehaviour
{
    [SerializeField]
    PushableBlock block;

    void OnTriggerEnter(Collider other)
    {
        block.PersonEntered(other);
    }
    void OnTriggerExit(Collider other)
    {
        block.PersonExited(other);
    }
}
