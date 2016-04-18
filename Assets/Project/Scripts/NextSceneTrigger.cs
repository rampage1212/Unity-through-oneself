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
            SceneTransitionManager scenes = Singleton.Get<SceneTransitionManager>();
            scenes.LoadNextLevel();
            GameSettings settings = Singleton.Get<GameSettings>();
            settings.NumLevelsUnlocked = (scenes.CurrentScene.Ordinal + GameSettings.DefaultNumLevelsUnlocked + 1);
            if(Debug.isDebugBuild == true)
            {
                Debug.Log("Set unlocked levels to: " + settings.NumLevelsUnlocked);
            }
            isTriggered = true;
        }
    }
}
