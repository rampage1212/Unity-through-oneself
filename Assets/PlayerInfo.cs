using UnityEngine;
using UnityStandardAssets.Characters.ThirdPerson;
using UnityStandardAssets.Cameras;

public class PlayerInfo : MonoBehaviour
{
    static PlayerInfo instance = null;

    [SerializeField]
    ThirdPersonUserControl playerController;
    [SerializeField]
    FreeLookCam cameraController;

    public static PlayerInfo Instance
    {
        get
        {
            return instance;
        }
    }

    public static ThirdPersonUserControl PlayerController
    {
        get
        {
            return instance.playerController;
        }
    }

    public static FreeLookCam CameraController
    {
        get
        {
            return instance.cameraController;
        }
    }

    void Awake()
    {
        instance = this;
    }

    void OnDestroy()
    {
        instance = null;
    }
}
