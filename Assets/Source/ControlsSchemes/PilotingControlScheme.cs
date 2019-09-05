using System.Collections.Generic;
using UnityEngine;

public class PilotingControlScheme : IControlScheme
{
    private const string PILOTING_CONTAINER_NAME = "PilotingContainer";
    private const float NAV_BOUNDS = 0.2f;
    private Transform pilotingContainer;

    public void Initialize(OVRPlayerController body, Transform camera, float sensitivity)
    {
        pilotingContainer = GameObject.Find(PILOTING_CONTAINER_NAME).transform;

        if (pilotingContainer != null)
        {
            Service.Controls.SetTouchObserver(OnTouchUpdate);
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

    public void OnTouchUpdate(TouchpadUpdate touchUpdate)
    {
        
    }
}
