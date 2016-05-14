using UnityEngine;
using OmiyaGames;

[RequireComponent(typeof(Collider))]
public class TutorialTrigger : MonoBehaviour
{
    [SerializeField]
    string defaultMessage;
    [SerializeField]
    int maxNumberOfTriggers = 0;
    [Header("Camera-Related Message")]
    [SerializeField]
    string cameraMessage;
    [SerializeField]
    Vector2 cameraYAngleRange = new Vector2(180f + 30f, 360f - 30f);
    [SerializeField]
    bool cameraInBetweenRange = true;

    ulong defaultPopUp = PopUpManager.InvalidId;
    ulong cameraPopUp = PopUpManager.InvalidId;
    MenuManager manager = null;
    bool isTriggered = false;
    int numTimesTriggered = 0;

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

    bool IsProperCameraAngle
    {
        get
        {
            bool returnFlag = false;
            float cameraAngle = PlayerInfo.CameraController.transform.rotation.eulerAngles.y;
            while (cameraAngle > 360f)
            {
                cameraAngle -= 360f;
            }
            while (cameraAngle < 0)
            {
                cameraAngle += 360f;
            }

            if(cameraInBetweenRange == true)
            {
                returnFlag = ((cameraAngle > cameraYAngleRange.x) && (cameraAngle < cameraYAngleRange.y));
            }
            else
            {
                returnFlag = ((cameraAngle < cameraYAngleRange.x) || (cameraAngle > cameraYAngleRange.y));
            }
            return returnFlag;
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if ((other.CompareTag("Player") == true) && (Manager.PopUps != null))
        {
            if ((string.IsNullOrEmpty(defaultMessage) == false) && (defaultPopUp == PopUpManager.InvalidId) && ((maxNumberOfTriggers < 1) || (numTimesTriggered < maxNumberOfTriggers)))
            {
                defaultPopUp = Manager.PopUps.ShowNewDialog(defaultMessage);
            }
            isTriggered = true;
            numTimesTriggered += 1;
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
            if (IsProperCameraAngle == true)
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
