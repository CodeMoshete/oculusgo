using System;
using System.Collections.Generic;

public class ShowLinearDialogueAction : CustomAction
{
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
        bool isConversationOver = dialogueIndex == Dialogues.Count - 1;
        Action onComplete = null;
        if (!isConversationOver)
        {
            onComplete = ShowNextDialogue;
        }
        else if (OnComplete != null)
        {
            onComplete = OnComplete.Initiate;
        }

        bool showDismiss = isConversationOver && 
            !(OnComplete is ShowBranchingDialogueAction) && 
            !(OnComplete is ShowLinearDialogueAction);

        // Service.Ui.ShowLinearDialogue(Dialogues[dialogueIndex], isConversationOver, showDismiss, onComplete);
        Service.EventManager.SendEvent(EventId.ShowDialogueText, Dialogues[dialogueIndex]);
        dialogueIndex++;
    }
}
