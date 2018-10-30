public class PromptTextAction : CustomAction
{
    public string PromptText;
    public float Duration;

    public override void Initiate()
    {
        PromptTextActionData evtData = new PromptTextActionData(PromptText, Duration);
        Service.EventManager.SendEvent(EventId.ShowPromptText, evtData);
    }
}
