using UnityEngine;
using UnityEngine.UI;
using Utils;

public class OculusGoControls : MonoBehaviour
{
    public float BackgroundCameraScale = 1f;

    public Text ConsoleLine;
	public float Sensitivity = 50f;

    [HideInInspector]
    public bool DisableMovement;

    private Transform backgroundCamera;
	private OVRPlayerController bodyObject;
	private Transform cameraObject;
	private Vector3 lastMousePos;
    private bool isDebugMenuActive;

    private Vector3 lastPlayerPosition;

	private bool isMouseCameraActive = true;
    private bool isJobsActive = true;

	void Start ()
    {
		bodyObject = GameObject.Find ("OVRPlayerController").GetComponent<OVRPlayerController>();
        backgroundCamera = GameObject.Find("OVRCameraRigBackground").transform;
        backgroundCamera.transform.localScale = 
            new Vector3(BackgroundCameraScale, BackgroundCameraScale, BackgroundCameraScale);
		cameraObject = UnityUtils.FindGameObject(bodyObject.gameObject, "TrackingSpace").transform;
        lastPlayerPosition = cameraObject.transform.position;

        UpdateManager manager = UpdateManager.Instance;
        Service.Controls.SetTouchObserver(TouchUpdate);
        Service.Controls.SetBackButtonObserver(BackUpdate);

        Service.EventManager.AddListener(EventId.SetControlsEnabled, OnControlsEnableSet);
    }

    private void OnDestroy()
    {
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

        Vector3 moveDist = (cameraObject.transform.position - lastPlayerPosition) * BackgroundCameraScale;
        backgroundCamera.localPosition += moveDist;
        backgroundCamera.localRotation = cameraObject.transform.rotation;
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
