using UnityEngine;

public class SwitchCollider : MonoBehaviour
{
    [SerializeField]
    Switch correspondingSwitch;

    void OnCollisionEnter(Collision info)
    {
        if(info.collider.CompareTag("Player") || info.collider.CompareTag("Helper"))
        {
            correspondingSwitch.AddPerson(info.collider);
        }
    }

    void OnCollisionExit(Collision info)
    {
        if (info.collider.CompareTag("Player") || info.collider.CompareTag("Helper"))
        {
            correspondingSwitch.RemovePerson(info.collider);
        }
    }
}
