using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OculusGoControls : MonoBehaviour
{
	private OVRPlayerController player;
	private GameObject ui;

	void Start () 
	{
		player = GameObject.Find ("OVRPlayerController").GetComponent<OVRPlayerController>();
		ui = GameObject.Find ("Canvas");
	}
	
	void Update () 
	{
		OVRInput.Controller activeController = OVRInput.GetActiveController();
		Vector2 primaryTouchpad = OVRInput.Get(OVRInput.Axis2D.PrimaryTouchpad);
		bool isPressed = OVRInput.Get (OVRInput.Button.One);
		player.MoveUp = isPressed && primaryTouchpad.y > 0.5;
		player.MoveDown = isPressed && primaryTouchpad.y < -0.5;
		player.MoveLeft = isPressed && primaryTouchpad.x < -0.5;
		player.MoveRight = isPressed && primaryTouchpad.x > 0.5;
		bool isTriggerPressed = OVRInput.GetDown (OVRInput.Button.PrimaryIndexTrigger);
		if (isTriggerPressed)
		{
			ui.SetActive (!ui.activeSelf);
		}
	}
}
