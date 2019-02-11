using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MultiDialoguePanel : MonoBehaviour
{
    private const string DEFAULT_OPTION_TEXT = "<Select an option using the thumb-pad...>";

    private readonly Color BUTTON_DEACTIVATED = new Color(1f, 0.698f, 0f, 0.663f);
    private readonly Color BUTTON_ACTIVATED = new Color(1f, 0.698f, 0f, 1f);

    private readonly Vector2 OPTION_1 = (new Vector2(1f, 1f)).normalized;
    private readonly Vector2 OPTION_2 = (new Vector2(1f, -1f)).normalized;
    private readonly Vector2 OPTION_3 = (new Vector2(-1f, -1f)).normalized;
    private readonly Vector2 OPTION_4 = (new Vector2(-1f, 1f)).normalized;

    public GameObject Panel;
    public Animator Animator;
    public Image ProfileImage;
    public GameObject PingEffect;
    public Text PromptText;
    public List<Image> OptionImages;
    public Text ResponseText;

    private bool isTransitioning;
    private bool isShowingDialogue;
    private int currentOptionIndex;
    private ShowBranchingDialogueAction actionData;

    public void Start()
    {
        Service.EventManager.AddListener(EventId.ShowChoiceDialogue, ShowMultiDialogue);
        Service.EventManager.AddListener(EventId.ChoiceDialogueDismissed, HideMultiDialogue);
    }

    private bool ShowMultiDialogue(object cookie)
    {
        if (!isShowingDialogue && !isTransitioning)
        {
            Service.Controls.SetTouchObserver(OnTouchUpdate);
            Service.Controls.SetTriggerObserver(OnTriggerUpdate);

            actionData = (ShowBranchingDialogueAction)cookie;
            Panel.SetActive(true);
            ProfileImage.sprite = actionData.ProfileImage;
            PromptText.text = actionData.Prompt;
            TriggerPing();
            Animator.SetBool("IsVisible", true);

            isShowingDialogue = true;
            isTransitioning = true;
            Service.TimerManager.CreateTimer(0.5f, TransitionInComplete, null);
        }
        return true;
    }

    private bool HideMultiDialogue(object cookie)
    {
        Service.Controls.RemoveTouchObserver(OnTouchUpdate);
        Service.Controls.RemoveTriggerObserver(OnTriggerUpdate);
        Animator.SetBool("IsVisible", false);
        Service.TimerManager.CreateTimer(0.5f, TransitionOutComplete, null);
        return true;
    }

    private void OnTouchUpdate(TouchpadUpdate update)
    {
        if (!update.TouchpadPressState && currentOptionIndex >= 0)
        {
            currentOptionIndex = -1;
            SetOptionHighlighted(currentOptionIndex);
        }

        if (Vector2.Dot(update.TouchpadPosition.normalized, OPTION_1) > 0.5f && 
            currentOptionIndex != 0)
        {
            SetOptionHighlighted(0);
        }
        else if (Vector2.Dot(update.TouchpadPosition.normalized, OPTION_2) > 0.5f && 
            currentOptionIndex != 1)
        {
            SetOptionHighlighted(1);
        }
        else if (Vector2.Dot(update.TouchpadPosition.normalized, OPTION_3) > 0.5f && 
            currentOptionIndex != 2)
        {
            SetOptionHighlighted(2);
        }
        else if (Vector2.Dot(update.TouchpadPosition.normalized, OPTION_4) > 0.5f && 
            currentOptionIndex != 3)
        {
            SetOptionHighlighted(3);
        }
    }

    private void SetOptionHighlighted(int optionIndex)
    {
        int numOptions = actionData.Options.Count;
        currentOptionIndex = optionIndex;

        if (optionIndex >= 0 && optionIndex < numOptions)
        {
            ResponseText.text = actionData.Options[optionIndex].OptionText;
        }
        else if (optionIndex == -1)
        {
            ResponseText.text = DEFAULT_OPTION_TEXT;
        }

        for (int i = 0; i < numOptions; ++i)
        {
            if (i == optionIndex)
            {
                OptionImages[i].color = BUTTON_ACTIVATED;
            }
            else
            {
                OptionImages[i].color = BUTTON_DEACTIVATED;
            }
        }
    }

    private void OnTriggerUpdate(TriggerUpdate update)
    {
        if (update.TriggerClicked && 
            currentOptionIndex >= 0 && 
            currentOptionIndex < actionData.Options.Count)
        {
            actionData.OnOptionSelected(currentOptionIndex);
        }
    }

    private void TransitionInComplete(object cookie)
    {
        isTransitioning = false;
    }

    private void TransitionOutComplete(object cookie)
    {
        isTransitioning = false;
        Panel.SetActive(false);
    }

    private void TriggerPing()
    {
        PingEffect.SetActive(false);
        PingEffect.SetActive(true);
    }
}
