using UnityEngine;
using OmiyaGames;
[RequireComponent(typeof(BoxCollider))]
public class NextSceneTrigger : MonoBehaviour
{
    bool isTriggered = false;

    void OnTriggerEnter(Collider other)
    {
        if((isTriggered == false) && (other.CompareTag("Player") == true))
        {
            Singleton.Get<SceneTransitionManager>().LoadNextLevel();
            isTriggered = true;
        }
    }
}
