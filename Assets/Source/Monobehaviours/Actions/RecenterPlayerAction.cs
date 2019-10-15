using UnityEngine;

public class RecenterPlayerAction : CustomAction
{
    public CustomAction OnDone;

    public override void Initiate()
    {
        if (!TryRecenterDisplay())
        {
            Service.UpdateManager.AddObserver(OnUpdate);
        }

        if (OnDone != null)
        {
            OnDone.Initiate();
        }
    }

    private bool TryRecenterDisplay()
    {
        UnityEngine.XR.InputTracking.Recenter();
        bool displayExists = OVRManager.display != null;
        if (displayExists)
        {
            OVRManager.display.RecenterPose();
        }
        return displayExists;
    }

    private void OnUpdate(float dt)
    {
        if (TryRecenterDisplay())
        {
            Debug.Log("Display recentered!");
            Service.UpdateManager.RemoveObserver(OnUpdate);
        }
    }
}
