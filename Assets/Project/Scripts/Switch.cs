using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(Animator))]
public class Switch : MonoBehaviour
{
    public enum State
    {
        Up = 0,
        Midway,
        Pressed
    }

    const string StateField = "state";
    const string LabelField = "showNumber";

    [Header("Switch Properties")]
    [SerializeField]
    int numberOfPeopleToActivate = 1;
    [SerializeField]
    bool activateOnce = true;

    [Header("Components")]
    [SerializeField]
    ParticleSystem pressedParticles = null;
    [SerializeField]
    UnityEngine.UI.Text label = null;

    static readonly System.Text.StringBuilder builder = new System.Text.StringBuilder();
    readonly HashSet<Collider> currentPeople = new HashSet<Collider>();
    Animator animatorCache = null;
    State currentState = State.Up;
    bool isLabelVisible = false;

    public Animator CachedAnimator
    {
        get
        {
            if(animatorCache == null)
            {
                animatorCache = GetComponent<Animator>();
            }
            return animatorCache;
        }
    }

    public State CurrentState
    {
        get
        {
            return currentState;
        }
        private set
        {
            if(currentState != value)
            {
                currentState = value;
                CachedAnimator.SetInteger(StateField, (int)currentState);
                if(currentState != State.Pressed)
                {
                    pressedParticles.Stop();
                }
            }
        }
    }

    bool IsLabelVisible
    {
        set
        {
            if(isLabelVisible != value)
            {
                isLabelVisible = value;
                CachedAnimator.SetBool(LabelField, isLabelVisible);
            }
        }
    }

    public void TriggerParticles()
    {
        pressedParticles.Play();
    }

    public bool AddPerson(Collider person)
    {
        bool returnFlag = false;
        if(currentPeople.Contains(person) == false)
        {
            currentPeople.Add(person);
            UpdateState();
            returnFlag = true;
        }
        return returnFlag;
    }

    public bool RemovePerson(Collider person)
    {
        bool returnFlag = false;
        if (currentPeople.Contains(person) == true)
        {
            currentPeople.Remove(person);
            UpdateState();
            returnFlag = true;
        }
        return returnFlag;
    }

    void UpdateState()
    {
        if (currentPeople.Count >= numberOfPeopleToActivate)
        {
            CurrentState = State.Pressed;
        }
        else if ((CurrentState != State.Pressed) || (activateOnce == false))
        {
            if (currentPeople.Count > 0)
            {
                CurrentState = State.Midway;
            }
            else
            {
                CurrentState = State.Up;
            }
        }
        Debug.Log("Num people: " + currentPeople.Count);

        // Build String
        builder.Length = 0;
        builder.Append(currentPeople.Count);
        builder.Append('/');
        builder.Append(numberOfPeopleToActivate);
        label.text = builder.ToString();

        if (CurrentState == State.Up)
        {
            IsLabelVisible = false;
        }
        else if((CurrentState == State.Pressed) && (activateOnce == true))
        {
            IsLabelVisible = false;
        }
        else
        {
            IsLabelVisible = true;
        }
    }
}
