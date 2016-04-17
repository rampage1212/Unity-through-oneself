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
    string groupName = "";

    [Header("Components")]
    [SerializeField]
    ParticleSystem pressedParticles = null;
    [SerializeField]
    UnityEngine.UI.Text label = null;
    [SerializeField]
    SwitchLine line = null;
    [SerializeField]
    OmiyaGames.SoundEffect pressedSound = null;
    [SerializeField]
    OmiyaGames.SoundEffect upSound = null;

    static readonly System.Text.StringBuilder builder = new System.Text.StringBuilder();
    static readonly Dictionary<string, HashSet<Switch>> groupedSwitches = new Dictionary<string, HashSet<Switch>>();
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
                    line.IsOn = false;
                }
                if(currentState == State.Up)
                {
                    upSound.Play();
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
        line.IsOn = true;
        pressedSound.Play();
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

    void ForceStateToUp()
    {
        CurrentState = State.Up;
        UpdateState();
    }

    void UpdateState()
    {
        if (currentPeople.Count >= numberOfPeopleToActivate)
        {
            CurrentState = State.Pressed;
            if(string.IsNullOrEmpty(groupName) == false)
            {
                HashSet<Switch> group;
                if (groupedSwitches.TryGetValue(groupName, out group) == true)
                {
                    foreach(Switch radioSwitch in group)
                    {
                        if(radioSwitch != this)
                        {
                            radioSwitch.ForceStateToUp();
                        }
                    }
                }
            }
        }
        else if (CurrentState != State.Pressed)
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

        // Build String
        builder.Length = 0;
        builder.Append(currentPeople.Count);
        builder.Append('/');
        builder.Append(numberOfPeopleToActivate);
        label.text = builder.ToString();

        if (CurrentState == State.Midway)
        {
            IsLabelVisible = true;
        }
        else
        {
            IsLabelVisible = false;
        }
    }

    void Start()
    {
        if(string.IsNullOrEmpty(groupName) == false)
        {
            // Grab group from the dictionary
            HashSet<Switch> group;
            if(groupedSwitches.TryGetValue(groupName, out group) == false)
            {
                // Add a new entry to the dictionary
                group = new HashSet<Switch>();
                groupedSwitches.Add(groupName, group);
            }

            // Add this switch into the list
            if(group.Contains(this) == false)
            {
                group.Add(this);
            }
        }
    }

    void OnDestroy()
    {
        if(groupedSwitches.Count > 0)
        {
            groupedSwitches.Clear();
        }
    }
}
