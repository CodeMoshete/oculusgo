using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Utils;

public class OculusGoControls : MonoBehaviour
{
    public float BackgroundCameraScale = 1f;

    public Text ConsoleLine;
	public GameObject Ui;
    public GameObject DebugUi;
	public float Sensitivity = 1f;

    private Transform backgroundCamera;
	private OVRPlayerController bodyObject;
	private Transform cameraObject;
	private Vector3 lastMousePos;

    private Vector3 lastPlayerPosition;

	private bool isMouseCameraActive;
    private bool isJobsActive = true;

    private UpdateManager updateManager;

	void Start ()
    {
		bodyObject = GameObject.Find ("OVRPlayerController").GetComponent<OVRPlayerController>();
        backgroundCamera = GameObject.Find("OVRCameraRigBackground").transform;
        backgroundCamera.transform.localScale = 
            new Vector3(BackgroundCameraScale, BackgroundCameraScale, BackgroundCameraScale);
		cameraObject = UnityUtils.FindGameObject(bodyObject.gameObject, "TrackingSpace").transform;
        lastPlayerPosition = cameraObject.transform.position;

        updateManager = Service.UpdateManager;

    }
	
	void Update () 
	{
        float dt = Time.deltaTime;

		OVRInput.Controller activeController = OVRInput.GetActiveController();
		Vector2 primaryTouchpad = OVRInput.Get(OVRInput.Axis2D.PrimaryTouchpad);
		bool isPressed = OVRInput.Get (OVRInput.Button.PrimaryTouchpad);

		bool MoveUp = isPressed && primaryTouchpad.y > 0.33 || Input.GetKey(KeyCode.W);
		bool MoveDown = isPressed && primaryTouchpad.y < -0.33 || Input.GetKey(KeyCode.S);
		bool MoveLeft = isPressed && primaryTouchpad.x < -0.33 || Input.GetKey(KeyCode.A);
		bool MoveRight = isPressed && primaryTouchpad.x > 0.33 || Input.GetKey(KeyCode.D);

        //Log("V: " + primaryTouchpad.y + ", H: " + primaryTouchpad.x + ", Dn: " + isPressed);

		if (MoveUp)
		{
			bodyObject.transform.Translate (new Vector3 (0f, 0f, 1f * dt));
		}
		else if (MoveDown)
		{
			bodyObject.transform.Translate (new Vector3 (0f, 0f, -1f * dt));
		}

		Vector3 euler = bodyObject.transform.eulerAngles;
		if (MoveLeft)
		{
			euler.y -= 75f * dt;
		}
		else if (MoveRight)
		{
			euler.y += 75f * dt;
		}
		bodyObject.transform.eulerAngles = euler;

		bool isTriggerPressed = OVRInput.GetDown (OVRInput.Button.PrimaryIndexTrigger);
        //Log("Trigger: " + isTriggerPressed);
		if (isTriggerPressed || Input.GetKeyDown(KeyCode.U))
		{
			DebugUi.SetActive (!DebugUi.activeSelf);
            Log("UI Active: " + DebugUi.activeSelf);
		}

        if (DebugUi.activeSelf && 
            (Input.GetKeyDown(KeyCode.J) || 
            (OVRInput.GetDown(OVRInput.Button.PrimaryTouchpad) && 
            Mathf.Abs(primaryTouchpad.y) < 0.33f &&
            Mathf.Abs(primaryTouchpad.x) < 0.33f)))
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

		if (isMouseCameraActive)
		{
			if (Input.GetMouseButtonDown (1))
			{
				lastMousePos = Input.mousePosition;
			}
			else
				if (Input.GetMouseButton (1))
				{
					Vector3 mouseDelta = lastMousePos - Input.mousePosition;
					lastMousePos = Input.mousePosition;
					euler = bodyObject.transform.eulerAngles;
					euler.y += dt * -mouseDelta.x * Sensitivity;
					bodyObject.transform.eulerAngles = euler;

					euler = cameraObject.eulerAngles;
					euler.x += dt * mouseDelta.y * Sensitivity;
					cameraObject.eulerAngles = euler;
				}
		}

        Vector3 moveDist = (cameraObject.transform.position - lastPlayerPosition) * BackgroundCameraScale;
        backgroundCamera.localPosition += moveDist;
        backgroundCamera.localRotation = cameraObject.transform.rotation;
        lastPlayerPosition = cameraObject.transform.position;
        //Debug.Log(backgroundCamera.eulerAngles.y + ", " + cameraObject.transform.eulerAngles.y);

        updateManager.Update(dt);
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
