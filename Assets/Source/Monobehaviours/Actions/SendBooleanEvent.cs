using UnityEngine;

public class SendBooleanEvent : CustomAction
{
    public EventId Event;
    public bool Value;

    public override void Initiate()
    {
        Service.EventManager.SendEvent(Event, Value);
    }
}
