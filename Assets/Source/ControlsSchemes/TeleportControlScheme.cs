using UnityEngine;

public class TeleportControlScheme : IControlScheme
{
    private bool disableMovement;
    private OVRPlayerController bodyObject;
    private Transform cameraObject;
    private bool hasPlayerTurned;
    private Vector3 lastMousePos;
    private float sensitivity;
    private bool isDebugMenuActive;

    public void Initialize(OVRPlayerController body, Transform camera, float sensitivity)
    {
        bodyObject = body;
        cameraObject = camera;
        this.sensitivity = sensitivity;

        Service.Controls.SetTouchObserver(TouchUpdate);
        Service.Controls.SetBackButtonObserver(BackUpdate);
    }

    public void Deactivate()
    {
        Service.Controls.RemoveTouchObserver(TouchUpdate);
        Service.Controls.RemoveBackButtonObserver(BackUpdate);
    }

    public void SetMovementEnabled(bool enabled)
    {
        disableMovement = !enabled;
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

        if (ControlsManager.Instance.CurrentHeadset == HeadsetModel.OculusQuest)
        {
            OVRInput.Controller activeController = OVRInput.GetActiveController();
        }

#if UNITY_EDITOR
        Vector3 euler = bodyObject.transform.eulerAngles;
        if (Input.GetMouseButtonDown(1))
        {
            lastMousePos = Input.mousePosition;
        }
        else if (Input.GetMouseButton(1))
        {
            Vector3 mouseDelta = lastMousePos - Input.mousePosition;
            lastMousePos = Input.mousePosition;
            euler = bodyObject.transform.eulerAngles;
            euler.y += dt * -((mouseDelta.x / Screen.width) * sensitivity);
            bodyObject.transform.eulerAngles = euler;

            euler = cameraObject.eulerAngles;
            euler.x += dt * (mouseDelta.y / Screen.height) * sensitivity;
            cameraObject.eulerAngles = euler;
        }
#endif
    }
}
