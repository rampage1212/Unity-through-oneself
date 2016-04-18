using UnityEngine;
using OmiyaGames;
using UnityStandardAssets.Characters.ThirdPerson;

[RequireComponent(typeof(ThirdPersonCharacter))]
public class CharacterSounds : MonoBehaviour
{
    static Vector3 vectorCache;

    [SerializeField]
    SoundEffect footsteps;
    [SerializeField]
    SoundEffect jump;
    [SerializeField]
    SoundEffect land;
    [SerializeField]
    float maximumDistanceBeforePlay = 4f;
    [SerializeField]
    float minFootstepDistance = 0.1f;

    // Use this for initialization
    void Start ()
    {
        ThirdPersonCharacter character = GetComponent<ThirdPersonCharacter>();
        character.OnFootStep += Character_OnFootStep;
        character.OnJump += Character_OnJump;
        character.OnLand += Character_OnLand;
    }

    private void Character_OnLand(ThirdPersonCharacter obj)
    {
        if(IsCloseEnoughToPlayer == true)
        {
            land.Play();
        }
    }

    private void Character_OnJump(ThirdPersonCharacter obj)
    {
        if (IsCloseEnoughToPlayer == true)
        {
            jump.Play();
        }
    }

    private void Character_OnFootStep(ThirdPersonCharacter obj)
    {
        if ((obj.ForwardAmount > minFootstepDistance) && (IsCloseEnoughToPlayer == true))
        {
            if(gameObject.name == "Controlled Character")
            {
                footsteps.Play();
            }
        }
    }

    bool IsCloseEnoughToPlayer
    {
        get
        {
            bool returnFlag = false;
            if (PlayerInfo.Instance != null)
            {
                if (transform != PlayerInfo.PlayerController.transform)
                {
                    // Check the distance
                    vectorCache = transform.position - PlayerInfo.PlayerController.transform.position;
                    if (vectorCache.sqrMagnitude < (maximumDistanceBeforePlay * maximumDistanceBeforePlay))
                    {
                        returnFlag = true;
                    }
                }
                else
                {
                    returnFlag = true;
                }
            }
            return returnFlag;
        }
    }
}
