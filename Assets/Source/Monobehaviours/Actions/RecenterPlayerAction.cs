using UnityEngine;

public class RecenterPlayerAction : CustomAction
{
    public CustomAction OnDone;
    public Transform OVRPlayerController;
    public float Rotation;
    private bool displayExistedLastFrame;

    public override void Initiate()
    {
#if UNITY_EDITOR
        RecenterDisplay();
#else
        Service.UpdateManager.AddObserver(OnUpdate);
#endif
        if (OnDone != null)
        {
            OnDone.Initiate();
        }
    }

    private void RecenterDisplay()
    {
        OVRPlayerController.localEulerAngles = new Vector3(0f, Rotation, 0f);
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
