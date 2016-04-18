using UnityEngine;
using OmiyaGames;

[RequireComponent(typeof(Collider))]
public class TutorialTrigger : MonoBehaviour
{
    [SerializeField]
    string defaultMessage;
    [SerializeField]
    string cameraMessage;
    [SerializeField]
    Vector2 cameraYAngleRange = new Vector2(180f + 30f, 360f - 30f);

    ulong defaultPopUp = PopUpManager.InvalidId;
    ulong cameraPopUp = PopUpManager.InvalidId;
    MenuManager manager = null;
    bool isTriggered = false;

    MenuManager Manager
    {
        get
        {
            if(manager == null)
            {
                manager = Singleton.Get<MenuManager>();
            }
            return manager;
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if ((other.CompareTag("Player") == true) && (Manager.PopUps != null))
        {
            isTriggered = true;
            if ((string.IsNullOrEmpty(defaultMessage) == false) && (defaultPopUp == PopUpManager.InvalidId))
            {
                defaultPopUp = Manager.PopUps.ShowNewDialog(defaultMessage);
            }
        }
    }

    void OnTriggerExit(Collider other)
    {
        if ((other.CompareTag("Player") == true) && (Manager.PopUps != null))
        {
            isTriggered = false;
            if (defaultPopUp != PopUpManager.InvalidId)
            {
                Manager.PopUps.RemoveDialog(defaultPopUp);
                defaultPopUp = PopUpManager.InvalidId;
            }
            if (cameraPopUp != PopUpManager.InvalidId)
            {
                Manager.PopUps.RemoveDialog(cameraPopUp);
                cameraPopUp = PopUpManager.InvalidId;
            }
        }
    }

    void Update()
    {
        if((isTriggered == true) && (string.IsNullOrEmpty(cameraMessage) == false))
        {
            float cameraAngle = PlayerInfo.CameraController.transform.rotation.eulerAngles.y;
            while (cameraAngle > 360f)
            {
                cameraAngle -= 360f;
            }
            while (cameraAngle < 0)
            {
                cameraAngle += 360f;
            }

            if ((cameraAngle > cameraYAngleRange.x) && (cameraAngle < cameraYAngleRange.y))
            {
                if(cameraPopUp == PopUpManager.InvalidId)
                {
                    cameraPopUp = Manager.PopUps.ShowNewDialog(cameraMessage);
                }
            }
            else
            {
                if (cameraPopUp != PopUpManager.InvalidId)
                {
                    Manager.PopUps.RemoveDialog(cameraPopUp);
                    cameraPopUp = PopUpManager.InvalidId;
                }
            }
        }
    }
}
