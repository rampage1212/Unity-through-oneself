using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using OmiyaGames;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(SoundEffect))]
public class PushableBlock : MonoBehaviour
{
    [SerializeField]
    int numberOfPeople = 1;
    [SerializeField]
    float heavyMass = 99999;
    [SerializeField]
    Text[] allLabels;

    [Header("Pitch Adjustment")]
    [SerializeField]
    Vector2 pitchRange = new Vector2(1f, 2f);
    [SerializeField]
    Vector2 volumeRange = new Vector2(0.1f, 0.8f);
    [SerializeField]
    Vector2 velocityToPitch = new Vector2(0.1f, 2f);
    [SerializeField]
    LayerMask groundLayerMask = ~0;
    [SerializeField]
    float m_GroundCheckDistance = 1f;

    [Header("Thud Sound")]
    [SerializeField]
    SoundEffect thud;
    [SerializeField]
    float playThudIfVelocityDifference = 1f;

    static readonly System.Text.StringBuilder builder = new System.Text.StringBuilder();
    readonly HashSet<Collider> currentPeople = new HashSet<Collider>();
    float originalMass = 0;
    Rigidbody body;
    SoundEffect sound;
    bool isPushable = false;
    float minVelocityForPitchSqr = 0;
    Vector3 horizontalVelocity;
    float lastYVelocity = 0f;

    bool IsPushable
    {
        get
        {
            return isPushable;
        }
        set
        {
            if(isPushable != value)
            {
                isPushable = value;
                if(isPushable == true)
                {
                    CachedBody.mass = originalMass;
                }
                else
                {
                    CachedBody.mass = heavyMass;
                }
            }
        }
    }

    Rigidbody CachedBody
    {
        get
        {
            if(body == null)
            {
                body = GetComponent<Rigidbody>();
                originalMass = body.mass;
            }
            return body;
        }
    }

    SoundEffect CachedSound
    {
        get
        {
            if(sound == null)
            {
                sound = GetComponent<SoundEffect>();
            }
            return sound;
        }
    }

    void Start()
    {
        // Square every velocity pitch parameters
        minVelocityForPitchSqr = (velocityToPitch.x * velocityToPitch.x);

        CachedBody.mass = heavyMass;

        UpdateLabels();
    }

    public void PersonEntered(Collider other)
    {
        if((RightPerson(other) == true) && (currentPeople.Contains(other) == false))
        {
            currentPeople.Add(other);
            UpdateLabels();
        }
    }

    public void PersonExited(Collider other)
    {
        if ((RightPerson(other) == true) && (currentPeople.Contains(other) == true))
        {
            currentPeople.Remove(other);
            UpdateLabels();
        }
    }

    void UpdateLabels()
    {
        builder.Length = 0;
        builder.Append(currentPeople.Count);
        builder.Append('/');
        builder.Append(numberOfPeople);

        string labelString = builder.ToString();
        foreach (Text label in allLabels)
        {
            label.text = labelString;
        }

        IsPushable = (currentPeople.Count >= numberOfPeople);
    }

    bool RightPerson(Collider other)
    {
        return ((other.CompareTag("Player") == true) || (other.CompareTag("Tall Helper")));
    }

    void Update()
    {
        // Play drag
        if (IsMovingAndOnGround(ref horizontalVelocity) == true)
        {
            // Play sound
            if(CachedSound.CurrentState != IAudio.State.Playing)
            {
                CachedSound.Play();
            }

            // Adjust pitch based on velocity
            float ratio = Mathf.InverseLerp(velocityToPitch.x, velocityToPitch.y, horizontalVelocity.magnitude);
            if(ratio > 1)
            {
                ratio = 1;
            }
            CachedSound.CenterPitch = Mathf.Lerp(pitchRange.x, pitchRange.y, ratio);
            CachedSound.CenterVolume = Mathf.Lerp(volumeRange.x, volumeRange.y, ratio);
        }
        else if(CachedSound.CurrentState != IAudio.State.Stopped)
        {
            // Stop looping sound
            CachedSound.CurrentState = IAudio.State.Stopped;
        }

        // Play thud
        if(Mathf.Abs(body.velocity.y - lastYVelocity) > playThudIfVelocityDifference)
        {
            thud.Play();
        }
        lastYVelocity = body.velocity.y;
    }

    bool IsMovingAndOnGround(ref Vector3 horizontalVelocity)
    {
        bool returnFlag = false;

        // Update horizontal velocity
        horizontalVelocity = body.velocity;
        horizontalVelocity.y = 0;

        if(horizontalVelocity.sqrMagnitude > minVelocityForPitchSqr)
        {
            RaycastHit hitInfo;
            if(Physics.Raycast(transform.position, Vector3.down, out hitInfo, m_GroundCheckDistance, groundLayerMask) == true)
            {
                returnFlag = true;
            }
        }
        return returnFlag;
    }
}
