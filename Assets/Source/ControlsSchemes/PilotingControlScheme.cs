using System.Collections.Generic;
using UnityEngine;

public class PilotingControlScheme : IControlScheme
{
    private const string PILOTING_CONTAINER_NAME = "PilotingContainer";
    private const float NAV_BOUNDS = 0.2f;
    private const float ACCELERATION = 0.01f;

    private Transform pilotingContainer;
    private bool disableMovement;
    private Vector2 containerPos;
    private Vector2 containerVel;

    public void Initialize(OVRPlayerController body, Transform camera, float sensitivity)
    {
        pilotingContainer = GameObject.Find(PILOTING_CONTAINER_NAME).transform;

        if (pilotingContainer != null)
        {
            Service.Controls.SetTouchObserver(OnTouchUpdate);
            Service.UpdateManager.AddObserver(OnUpdate);
        }
        else
        {
            Debug.LogError("[PilotingControlScheme] Could not find piloting container object!");
        }
    }

    public void SetMovementEnabled(bool enabled)
    {

    }

    public void Deactivate()
    {

    }

    private void OnTouchUpdate(TouchpadUpdate update)
    {
        bool isPressed = update.TouchpadPressState;
        if (isPressed)
        {
            Vector2 touchDir = update.TouchpadPosition;
            float accel = touchDir.magnitude * ACCELERATION;
            containerVel += touchDir * accel;
        }
    }

    private void OnUpdate(float dt)
    {
        containerPos += containerVel;
        pilotingContainer.localPosition = containerPos;
    }
}
