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

    static readonly System.Text.StringBuilder builder = new System.Text.StringBuilder();
    readonly HashSet<Collider> currentPeople = new HashSet<Collider>();
    float originalMass = 0;
    Rigidbody body;
    SoundEffect sound;
    bool isPushable = false;
    float minVelocityForPitchSqr = 0;

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
        if(body.velocity.sqrMagnitude > minVelocityForPitchSqr)
        {
            // Play sound
            if(CachedSound.CurrentState != IAudio.State.Playing)
            {
                CachedSound.Play();
                //Debug.Log("Play drag sound");
            }

            // Adjust pitch based on velocity
            float ratio = Mathf.InverseLerp(velocityToPitch.x, velocityToPitch.y, body.velocity.magnitude);
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
            Debug.Log("Stop drag sound");
        }
    }
}
