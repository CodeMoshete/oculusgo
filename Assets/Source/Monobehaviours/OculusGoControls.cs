using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utils;

public class OculusGoControls : MonoBehaviour
{
	public float Sensitivity = 1f;

	private OVRPlayerController bodyObject;
	private Transform cameraObject;
	private Vector3 lastMousePos;
	private GameObject ui;

	private bool isMouseCameraActive;

	void Start () 
	{
		bodyObject = GameObject.Find ("OVRPlayerController").GetComponent<OVRPlayerController>();
		cameraObject = UnityUtils.FindGameObject(bodyObject.gameObject, "OVRCameraRig").transform;
		ui = GameObject.Find ("Canvas");
	}
	
	void Update () 
	{
		OVRInput.Controller activeController = OVRInput.GetActiveController();
		Vector2 primaryTouchpad = OVRInput.Get(OVRInput.Axis2D.PrimaryTouchpad);
		bool isPressed = OVRInput.Get (OVRInput.Button.One);

		bool MoveUp = isPressed && primaryTouchpad.y > 1 || Input.GetKey(KeyCode.W);
		bool MoveDown = isPressed && primaryTouchpad.y < -1 || Input.GetKey(KeyCode.S);
		bool MoveLeft = isPressed && primaryTouchpad.x < -1 || Input.GetKey(KeyCode.A);
		bool MoveRight = isPressed && primaryTouchpad.x > 1 || Input.GetKey(KeyCode.D);

		if (MoveUp)
		{
			bodyObject.transform.Translate (new Vector3 (0f, 0f, 1f * Time.deltaTime));
		}
		else if (MoveDown)
		{
			bodyObject.transform.Translate (new Vector3 (0f, 0f, -1f * Time.deltaTime));
		}

		Vector3 euler = bodyObject.transform.eulerAngles;
		if (MoveLeft)
		{
			euler.y -= 75f * Time.deltaTime;
		}
		else if (MoveRight)
		{
			euler.y += 75f * Time.deltaTime;
		}
		bodyObject.transform.eulerAngles = euler;

		bool isTriggerPressed = OVRInput.GetDown (OVRInput.Button.PrimaryIndexTrigger);
		if (isTriggerPressed || Input.GetKeyDown(KeyCode.U))
		{
			ui.SetActive (!ui.activeSelf);
		}

		if (Input.GetKey (KeyCode.LeftControl) && Input.GetKeyDown (KeyCode.D))
		{
			isMouseCameraActive = !isMouseCameraActive;
			Debug.Log ("Mouse camera controls active: " + isMouseCameraActive);
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
					float dt = Time.deltaTime;
					euler = bodyObject.transform.eulerAngles;
					euler.y += dt * -mouseDelta.x * Sensitivity;
					bodyObject.transform.eulerAngles = euler;

					euler = cameraObject.eulerAngles;
					euler.x += dt * mouseDelta.y * Sensitivity;
					cameraObject.eulerAngles = euler;
				}
		}
	}
}
