using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ControlsType
{
    Trigger,
    Touchpad,
    BackButton
}

public struct TouchpadUpdate
{
    public Vector2 TouchpadPosition;
    public bool TouchpadPressState;
    public bool TouchpadClicked;
}

public struct TriggerUpdate
{
    public bool TriggerPressState;
    public bool TriggerClicked;
}

public struct BackButtonUpdate
{
    public bool BackButtonPressState;
    public bool BackButtonClicked;
}

public class ControlsManager
{
    private List<Action<TouchpadUpdate>> touchpadListeners;
    private List<Action<TriggerUpdate>> triggerListeners;
    private List<Action<BackButtonUpdate>> backButtonListeners;

    private static ControlsManager instance;
    public static ControlsManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new ControlsManager();
            }
            return instance;
        }
    }

    public ControlsManager()
    {
        touchpadListeners = new List<Action<TouchpadUpdate>>();
        triggerListeners = new List<Action<TriggerUpdate>>();
        backButtonListeners = new List<Action<BackButtonUpdate>>();
        Service.UpdateManager.AddObserver(Update);
    }

    private void Update(float dt)
    {
        OVRInput.Controller activeController = OVRInput.GetActiveController();
        TouchpadUpdate touchUpdate = new TouchpadUpdate();
        touchUpdate.TouchpadPosition = OVRInput.Get(OVRInput.Axis2D.PrimaryTouchpad);
        touchUpdate.TouchpadPressState = OVRInput.Get(OVRInput.Button.PrimaryTouchpad);
        touchUpdate.TouchpadClicked = OVRInput.GetDown(OVRInput.Button.PrimaryTouchpad);
        touchpadListeners[touchpadListeners.Count - 1](touchUpdate);

        TriggerUpdate triggerUpdate = new TriggerUpdate();
        triggerUpdate.TriggerPressState = OVRInput.Get(OVRInput.Button.PrimaryIndexTrigger);
        triggerUpdate.TriggerClicked = OVRInput.GetDown(OVRInput.Button.PrimaryIndexTrigger);
        triggerListeners[triggerListeners.Count - 1](triggerUpdate);

        BackButtonUpdate backButtonUpdate = new BackButtonUpdate();
        backButtonUpdate.BackButtonPressState = OVRInput.Get(OVRInput.Button.Back);
        backButtonUpdate.BackButtonClicked = OVRInput.GetDown(OVRInput.Button.Back);
        backButtonListeners[backButtonListeners.Count - 1](backButtonUpdate);
    }

    public void SetTouchObserver(Action<TouchpadUpdate> callback)
    {
        touchpadListeners.Add(callback);
    }

    public void RemoveTouchObserver(Action<TouchpadUpdate> callback)
    {
        touchpadListeners.Remove(callback);
    }

    public void SetTriggerObserver(Action<TriggerUpdate> callback)
    {
        triggerListeners.Add(callback);
    }

    public void RemoveTriggerObserver(Action<TriggerUpdate> callback)
    {
        triggerListeners.Remove(callback);
    }

    public void SetBackButtonObserver(Action<BackButtonUpdate> callback)
    {
        backButtonListeners.Add(callback);
    }

    public void RemoveBackButtonObserver(Action<BackButtonUpdate> callback)
    {
        backButtonListeners.Remove(callback);
    }
}
