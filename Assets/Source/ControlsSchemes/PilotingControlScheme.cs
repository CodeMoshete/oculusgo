﻿using UnityEngine;

public class PilotingControlScheme : IControlScheme
{
    private const string PILOTING_CONTAINER_NAME = "PilotingContainer";
    private const float ACCELERATION = 0.01f;
    private const float DRAG = 0.92f;
    private const float MAX_SPEED_SQR = 0.0225f;
    private const float MAX_SPEED = 0.15f;
    private const float MAX_DIST_FROM_CENTER_SQR = 0.0225f;
    private const float MAX_CAM_TILT = 7.5f;

    private Transform pilotingContainer;
    private bool disableMovement;
    private bool isSteering;
    private Vector2 containerPos;
    private Vector2 containerVel;
    private float sqrDistFromCenter;

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
            touchDir.x *= -1f;
            float accel = touchDir.magnitude * ACCELERATION;

            bool isInBounds = sqrDistFromCenter < MAX_DIST_FROM_CENTER_SQR;

            bool applyX = isInBounds ||
                (touchDir.x < 0 && containerPos.x > 0) || 
                (touchDir.x > 0 && containerPos.x < 0);

            bool applyY = isInBounds ||
                (touchDir.y < 0 && containerPos.y > 0) ||
                (touchDir.y > 0 && containerPos.y < 0);

            isSteering = applyX || applyY;

            if (applyX)
            {
                containerVel.x += touchDir.x * accel;
            }

            if (applyY)
            {
                containerVel.y += touchDir.y * accel;
            }

            if (containerVel.sqrMagnitude > MAX_SPEED_SQR)
            {
                containerVel = containerVel.normalized * MAX_SPEED;
            }
        }
        else
        {
            isSteering = false;
        }
    }

    private void OnUpdate(float dt)
    {
        if (!isSteering)
        {
            containerVel *= (1f - DRAG * dt);
        }

        containerPos += containerVel * dt;
        sqrDistFromCenter = containerPos.sqrMagnitude;

        pilotingContainer.localPosition = containerPos;
        Vector3 euler = pilotingContainer.localEulerAngles;
        euler.z = -containerVel.x / MAX_SPEED * MAX_CAM_TILT;
        euler.x = containerVel.y / MAX_SPEED * MAX_CAM_TILT;
        pilotingContainer.localEulerAngles = euler;
    }
}
