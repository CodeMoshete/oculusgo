using System.Collections;
using UnityEngine;
using Utils;

public class RecenterPlayerAction : CustomAction
{
    public CustomAction OnDone;
    public Transform OVRPlayerController;
    private Transform centerEyeAnchor;
    private Vector3 initialRotation;
    public float Rotation;
    private bool displayExistedLastFrame;
    private bool wasCameraUpdated;

    public override void Initiate()
    {
        OVRPlayerController.localEulerAngles = new Vector3(0f, Rotation, 0f);
        initialRotation = OVRPlayerController.localEulerAngles;

#if UNITY_EDITOR
        RecenterDisplay();
        wasCameraUpdated = true;
#else
        OVRPlayerController.GetComponent<OVRPlayerController>().CameraUpdated += OnCameraUpdated;
        Service.UpdateManager.AddObserver(OnUpdate);
#endif
        if (OnDone != null)
        {
            OnDone.Initiate();
        }
    }

    private void OnCameraUpdated()
    {
        wasCameraUpdated = true;
    }

    private void RecenterDisplay()
    {
        StartCoroutine(ResetCamera());
    }

    IEnumerator ResetCamera()
    {
        while (OVRPlayerController.localEulerAngles == initialRotation && !wasCameraUpdated)
        {
            yield return null;
        }
        yield return new WaitForEndOfFrame();
        OVRPlayerController.localEulerAngles = new Vector3(0f, Rotation, 0f);
        Debug.Log("Camera position reset!");
    }

    private void OnUpdate(float dt)
    {
        if (displayExistedLastFrame)
        {
            RecenterDisplay();
            Service.UpdateManager.RemoveObserver(OnUpdate);
        }

        displayExistedLastFrame = OVRManager.display != null;
    }
}
