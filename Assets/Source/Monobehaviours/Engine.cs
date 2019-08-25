using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Utils;

public class Engine : MonoBehaviour
{
    [System.Serializable]
    public struct PlayerCameraMirror
    {
        public Transform MirrorCamera;
        public float MovementScale;
    }

    public float BackgroundCameraScale = 1f;

    public List<PlayerCameraMirror> MirrorCameras;

    public Text ConsoleLine;
	public float Sensitivity = 50f;

    public Transform HeadCamera;
    public float OculusGoCameraHeight;
    public float OculusQuestCameraHeight;

    private Dictionary<ControlScheme, IControlScheme> controlSchemes;
    private IControlScheme currentControlScheme;

    private Transform backgroundCamera;
	private OVRPlayerController bodyObject;
	private Transform cameraObject;
    private bool isDebugMenuActive;

    private Vector3 lastPlayerPosition;

    private bool isJobsActive = true;

	void Start ()
    {
		bodyObject = GameObject.Find ("OVRPlayerController").GetComponent<OVRPlayerController>();
        for (int i = 0, count = MirrorCameras.Count; i < count; ++i)
        {
            float scale = MirrorCameras[i].MovementScale;
            MirrorCameras[i].MirrorCamera.localScale = new Vector3(scale, scale, scale);
        }

		cameraObject = UnityUtils.FindGameObject(bodyObject.gameObject, "TrackingSpace").transform;
        lastPlayerPosition = cameraObject.transform.position;

        UpdateManager manager = UpdateManager.Instance;

        controlSchemes = new Dictionary<ControlScheme, IControlScheme>();
        controlSchemes.Add(ControlScheme.Movement, new MovementControlScheme());
        controlSchemes.Add(ControlScheme.Teleport, new TeleportControlScheme());
        SetControlScheme(ControlScheme.Teleport);

        Vector3 cameraStartPos = HeadCamera.localPosition;
        switch(Service.Controls.CurrentHeadset)
        {
            case HeadsetModel.OculusGo:
                cameraStartPos.y = OculusGoCameraHeight;
                break;
            case HeadsetModel.OculusQuest:
                cameraStartPos.y = OculusQuestCameraHeight;
                break;
        }
        HeadCamera.localPosition = cameraStartPos;

        Service.EventManager.AddListener(EventId.SetControlsEnabled, OnControlsEnableSet);

        Log("Device name: " + SystemInfo.deviceName + 
            "\nDevice model: " + SystemInfo.deviceModel +
            "\nDevice Type: " + SystemInfo.deviceType);
    }

    private void OnDestroy()
    {
        Service.EventManager.RemoveListener(EventId.SetControlsEnabled, OnControlsEnableSet);
        currentControlScheme.Deactivate();
    }

    private void SetControlScheme(ControlScheme type)
    {
        IControlScheme nextScheme = controlSchemes[type];
        if (nextScheme != currentControlScheme && currentControlScheme != null)
        {
            currentControlScheme.Deactivate();
        }
        currentControlScheme = nextScheme;
        currentControlScheme.Initialize(bodyObject, cameraObject, Sensitivity);
    }

    public bool OnControlsEnableSet(object cookie)
    {
        currentControlScheme.SetMovementEnabled((bool)cookie);
        return true;
    }

	void Update () 
	{
        float dt = Time.deltaTime;

        if (Input.GetKeyDown(KeyCode.U))
        {
            isDebugMenuActive = !isDebugMenuActive;
            Service.EventManager.SendEvent(EventId.DebugToggleConsole, isDebugMenuActive);
		}

        if (isDebugMenuActive && Input.GetKeyDown(KeyCode.J))
        {
            isJobsActive = !isJobsActive;
            Log("Jobs activated: " + isJobsActive);
            Service.EventManager.SendEvent(EventId.DebugToggleJobs, isJobsActive);
        }

        for (int i = 0, count = MirrorCameras.Count; i < count; ++i)
        {
            float scale = MirrorCameras[i].MovementScale;
            Vector3 moveDist = (cameraObject.transform.position - lastPlayerPosition) * scale;
            MirrorCameras[i].MirrorCamera.localPosition += moveDist;
            MirrorCameras[i].MirrorCamera.localRotation = cameraObject.transform.rotation;
        }
        lastPlayerPosition = cameraObject.transform.position;
    }

    private void Log(string message)
    {
        if (ConsoleLine != null)
        {
            ConsoleLine.text = message;
        }
        Debug.Log(message);
    }
}
