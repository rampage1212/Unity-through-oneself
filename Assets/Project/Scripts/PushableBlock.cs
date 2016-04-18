using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

[RequireComponent(typeof(Rigidbody))]
public class PushableBlock : MonoBehaviour
{
    [SerializeField]
    int numberOfPeople = 1;
    [SerializeField]
    float heavyMass = 99999;
    [SerializeField]
    Text[] allLabels;

    static readonly System.Text.StringBuilder builder = new System.Text.StringBuilder();
    readonly HashSet<Collider> currentPeople = new HashSet<Collider>();
    float originalMass = 0;
    Rigidbody body;
    bool isPushable = false;

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

    void Start()
    {
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
}
