using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Utils;

public class OculusGoControls : MonoBehaviour
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

    [HideInInspector]
    public bool DisableMovement;

    public Transform HeadCamera;
    public float OculusGoCameraHeight;
    public float OculusQuestCameraHeight;

    private Transform backgroundCamera;
	private OVRPlayerController bodyObject;
	private Transform cameraObject;
	private Vector3 lastMousePos;
    private bool isDebugMenuActive;

    private Vector3 lastPlayerPosition;
    private bool hasPlayerTurned;

	private bool isMouseCameraActive = true;
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
        Service.Controls.SetTouchObserver(TouchUpdate);
        Service.Controls.SetBackButtonObserver(BackUpdate);

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
        Service.Controls.RemoveTouchObserver(TouchUpdate);
        Service.Controls.RemoveBackButtonObserver(BackUpdate);
    }

    public bool OnControlsEnableSet(object cookie)
    {
        DisableMovement = (bool)cookie;
        return true;
    }

    private void BackUpdate(BackButtonUpdate update)
    {
        if (update.BackButtonClicked)
        {
            isDebugMenuActive = !isDebugMenuActive;
            Service.EventManager.SendEvent(EventId.DebugToggleConsole, isDebugMenuActive);
        }
    }

    private void TouchUpdate(TouchpadUpdate update)
    {
        float dt = Time.deltaTime;

        bool isPressed = update.TouchpadPressState;
        bool isPressedThisFrame = update.TouchpadClicked;
        bool MoveUp = isPressed && !DisableMovement && update.TouchpadPosition.y > 0.33;
        bool MoveDown = isPressed && !DisableMovement && update.TouchpadPosition.y < -0.33;

        if (ControlsManager.Instance.CurrentHeadset == HeadsetModel.OculusQuest)
        {
            OVRInput.Controller activeController = OVRInput.GetActiveController();
            Vector2 thumbPos = OVRInput.Get(OVRInput.RawAxis2D.RThumbstick, activeController);
            if (!hasPlayerTurned && Mathf.Abs(update.TouchpadPosition.x) > 0.7)
            {
                isPressedThisFrame = true;
                hasPlayerTurned = true;
            }
            else if (hasPlayerTurned && Mathf.Abs(update.TouchpadPosition.x) <= 0.7)
            {
                hasPlayerTurned = false;
            }
        }

        bool MoveLeft = isPressedThisFrame && update.TouchpadPosition.x < -0.33;
        bool MoveRight = isPressedThisFrame && update.TouchpadPosition.x > 0.33;

        //Log("V: " + primaryTouchpad.y + ", H: " + primaryTouchpad.x + ", Dn: " + isPressed);

        if (MoveUp)
        {
            bodyObject.transform.Translate(new Vector3(0f, 0f, 1f * dt));
        }
        else if (MoveDown)
        {
            bodyObject.transform.Translate(new Vector3(0f, 0f, -1f * dt));
        }

        Vector3 euler = bodyObject.transform.eulerAngles;
        if (!MoveUp && !MoveDown)
        {
            if (MoveLeft)
            {
                // euler.y -= 75f * dt;
                euler.y -= 45f;
            }
            else if (MoveRight)
            {
                // euler.y += 75f * dt;
                euler.y += 45f;
            }
            bodyObject.transform.eulerAngles = euler;
        }

#if UNITY_EDITOR
        if (isMouseCameraActive)
        {
            if (Input.GetMouseButtonDown(1))
            {
                lastMousePos = Input.mousePosition;
            }
            else if (Input.GetMouseButton(1))
            {
                Vector3 mouseDelta = lastMousePos - Input.mousePosition;
                lastMousePos = Input.mousePosition;
                euler = bodyObject.transform.eulerAngles;
                euler.y += dt * -((mouseDelta.x / Screen.width) * Sensitivity);
                bodyObject.transform.eulerAngles = euler;

                euler = cameraObject.eulerAngles;
                euler.x += dt * (mouseDelta.y / Screen.height) * Sensitivity;
                cameraObject.eulerAngles = euler;
            }
        }
#endif
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

        if (Input.GetKey (KeyCode.LeftControl) && Input.GetKeyDown (KeyCode.D))
		{
			isMouseCameraActive = !isMouseCameraActive;
            Debug.Log("Debug controls active: " + isMouseCameraActive);
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
