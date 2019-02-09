using System;
using System.Collections.Generic;

[Serializable]
public class DialogueOption
{
    public string OptionText;
    public CustomAction OnSelected;

    public DialogueOption(string optionTxt, CustomAction onSelected)
    {
        OptionText = optionTxt;
        OnSelected = onSelected;
    }
}

public class ShowBranchingDialogueAction : CustomAction
{
    public string Prompt;
    public List<DialogueOption> Options;

    public override void Initiate()
    {
        // Service.Ui.ShowBranchingDialogue(Prompt, Options);
    }
}
