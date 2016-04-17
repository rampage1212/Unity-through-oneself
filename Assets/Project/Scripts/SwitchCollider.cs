using UnityEngine;

public class SwitchCollider : MonoBehaviour
{
    [SerializeField]
    Switch correspondingSwitch;

    void OnCollisionEnter(Collision info)
    {
        if (Trigger(info.collider) == true)
        {
            correspondingSwitch.AddPerson(info.collider);
        }
    }

    void OnCollisionExit(Collision info)
    {
        if (Trigger(info.collider) == true)
        {
            correspondingSwitch.RemovePerson(info.collider);
        }
    }

    bool Trigger(Collider other)
    {
        return ((other.CompareTag("Player") || other.CompareTag("Short Helper") || other.CompareTag("Tall Helper")));
    }
}
