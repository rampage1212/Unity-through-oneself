using UnityEngine;
using System.Collections.Generic;
using OmiyaGames;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(SoundEffect))]
public class Door : MonoBehaviour
{
    const string OpenField = "open";

    [SerializeField]
    bool openByDefault = false;
    [SerializeField]
    Switch[] allSwitches = new Switch[1];

    SoundEffect sound = null;
    Animator animator = null;
    bool isOpen;
    readonly HashSet<Switch> allUniqueSwitches = new HashSet<Switch>();

    public Animator CachedAnimator
    {
        get
        {
            if(animator == null)
            {
                animator = GetComponent<Animator>();
            }
            return animator;
        }
    }

    public SoundEffect CachedSound
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

    public bool IsOpen
    {
        get
        {
            return isOpen;
        }
        private set
        {
            if(isOpen != value)
            {
                isOpen = value;
                CachedAnimator.SetBool(OpenField, isOpen);
                CachedSound.Play();
            }
        }
    }

    // Use this for initialization
    void Start ()
    {
        // Setup flags
        isOpen = openByDefault;
        CachedAnimator.SetBool(OpenField, isOpen);

        // Setup switches
        allUniqueSwitches.Clear();
        foreach (Switch button in allSwitches)
        {
            if((button != null) && (allUniqueSwitches.Contains(button) == false))
            {
                button.OnAfterStateChanged += Button_OnAfterStateChanged;
                allUniqueSwitches.Add(button);
            }
        }
    }

    private void Button_OnAfterStateChanged(Switch arg1, Switch.State arg2)
    {
        if (allUniqueSwitches.Count > 0)
        {
            int numPressed = 0;
            foreach (Switch button in allUniqueSwitches)
            {
                if (button.CurrentState == Switch.State.Pressed)
                {
                    numPressed += 1;
                }
            }

            if (numPressed >= allUniqueSwitches.Count)
            {
                IsOpen = !openByDefault;
            }
            else
            {
                IsOpen = openByDefault;
            }
        }
    }
}
