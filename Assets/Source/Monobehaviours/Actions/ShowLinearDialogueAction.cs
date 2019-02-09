using System.Collections.Generic;

public class ShowLinearDialogueAction : CustomAction
{
    public bool LockMovement;
    public float TimeDelay;
    public List<string> Dialogues;
    public CustomAction OnComplete;

    private int dialogueIndex;

    public override void Initiate()
    {
        dialogueIndex = 0;
        ShowNextDialogue();
    }

    private void ShowNextDialogue()
    {
        bool isLastDialogue = dialogueIndex == Dialogues.Count - 1;

        if (dialogueIndex == 0)
        {
            if (LockMovement)
            {
                Service.Controls.SetTouchObserver(OnTouch);
            }
            Service.Controls.SetTriggerObserver(OnTrigger);
        }

        if (TimeDelay > 0f)
        {
            Service.TimerManager.CreateTimer(TimeDelay, OnTimeExpired, null);
        }

        Service.EventManager.SendEvent(EventId.ShowDialogueText, Dialogues[dialogueIndex]);
        dialogueIndex++;
    }

    private void OnTouch(TouchpadUpdate update)
    {
        // Intentionally empty.
    }

    private void OnTrigger(TriggerUpdate update)
    {
        if (update.TriggerClicked && TimeDelay <= 0)
        {
            TriggerContinue();
        }
    }

    private void OnTimeExpired(object cookie)
    {
        TriggerContinue();
    }

    private void TriggerContinue()
    {
        bool isDone = dialogueIndex == Dialogues.Count;

        if (isDone)
        {
            Service.Controls.RemoveTouchObserver(OnTouch);
            Service.Controls.RemoveTriggerObserver(OnTrigger);

            if (isDone &&
                !(OnComplete is ShowBranchingDialogueAction) &&
                !(OnComplete is ShowLinearDialogueAction))
            {
                Service.EventManager.SendEvent(EventId.DialogueTextDismissed, null);
            }

            if (OnComplete != null)
            {
                OnComplete.Initiate();
            }
        }
        else
        {
            ShowNextDialogue();
        }
    }
}
