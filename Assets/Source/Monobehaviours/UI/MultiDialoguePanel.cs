using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MultiDialoguePanel : MonoBehaviour
{
    private const string DEFAULT_OPTION_TEXT = "<Select an option using the thumb-pad...>";
    private const float SELECT_FILL_TIME = 3f;

    private readonly Color BUTTON_DEACTIVATED = new Color(1f, 0.698f, 0f, 0.663f);
    private readonly Color BUTTON_ACTIVATED = new Color(1f, 0.698f, 0f, 1f);

    private readonly Vector2 OPTION_1 = Vector2.up;
    private readonly Vector2 OPTION_2 = Vector2.right;
    private readonly Vector2 OPTION_3 = Vector2.down;
    private readonly Vector2 OPTION_4 = Vector2.left;

    public GameObject Panel;
    public Animator Animator;
    public Image ProfileImage;
    public GameObject PingEffect;
    public TextReveal PromptText;
    public List<Image> OptionImages;
    public Text ResponseText;
    public Image SelectionFill;

    private bool isTransitioning;
    private bool isShowingDialogue;

    private float optionSelectPct;
    private int currentOptionIndex;
    private bool isOptionSelectingTouch;
    private bool isOptionSelectingTrigger;
    private bool isOptionSelecting
    {
        get
        {
            return isOptionSelectingTouch || isOptionSelectingTrigger;
        }
    }

    private ShowBranchingDialogueAction actionData;

    public void Start()
    {
        Service.EventManager.AddListener(EventId.ShowChoiceDialogue, ShowMultiDialogue);
        Service.EventManager.AddListener(EventId.ChoiceDialogueDismissed, HideMultiDialogue);
    }

    public void OnDestroy()
    {
        Service.EventManager.RemoveListener(EventId.ShowChoiceDialogue, ShowMultiDialogue);
        Service.EventManager.RemoveListener(EventId.ChoiceDialogueDismissed, HideMultiDialogue);
    }

    private bool ShowMultiDialogue(object cookie)
    {
        if (!isShowingDialogue && !isTransitioning)
        {
            Service.Controls.SetTouchObserver(OnTouchUpdate);
            Service.Controls.SetTriggerObserver(OnTriggerUpdate);
            Service.UpdateManager.AddObserver(OnUpdate);

            actionData = (ShowBranchingDialogueAction)cookie;
            Panel.SetActive(true);
            ProfileImage.sprite = actionData.ProfileImage;
            PromptText.text = actionData.Prompt;
            PromptText.OnShowComplete = PromptTextDisplayed;
            TriggerPing();
            Animator.SetBool("IsVisible", true);

            isShowingDialogue = true;
            isTransitioning = true;
            Service.TimerManager.CreateTimer(0.5f, TransitionInComplete, null);
        }
        return true;
    }

    private void PromptTextDisplayed()
    {
        Service.TimerManager.CreateTimer(1f, ShowChoice, null);
    }

    private void ShowChoice(object cookie)
    {
        Animator.SetBool("IsPlayerChoiceVisible", true);
    }

    private bool HideMultiDialogue(object cookie)
    {
        Service.Controls.RemoveTouchObserver(OnTouchUpdate);
        Service.Controls.RemoveTriggerObserver(OnTriggerUpdate);
        Service.UpdateManager.RemoveObserver(OnUpdate);
        Animator.SetBool("IsVisible", false);
        Service.TimerManager.CreateTimer(0.5f, TransitionOutComplete, null);
        return true;
    }

    private void OnTouchUpdate(TouchpadUpdate update)
    {
        int prevOptionIndex = currentOptionIndex;

        if (update.TouchpadPosition == Vector2.zero && currentOptionIndex >= 0)
        {
            currentOptionIndex = -1;
            SetOptionHighlighted(currentOptionIndex);
        }
        else if (Vector2.Dot(update.TouchpadPosition.normalized, OPTION_1) > 0.75f && 
            currentOptionIndex != 0)
        {
            SetOptionHighlighted(0);
        }
        else if (Vector2.Dot(update.TouchpadPosition.normalized, OPTION_2) > 0.75f && 
            currentOptionIndex != 1)
        {
            SetOptionHighlighted(1);
        }
        else if (Vector2.Dot(update.TouchpadPosition.normalized, OPTION_3) > 0.75f && 
            currentOptionIndex != 2)
        {
            SetOptionHighlighted(2);
        }
        else if (Vector2.Dot(update.TouchpadPosition.normalized, OPTION_4) > 0.75f && 
            currentOptionIndex != 3)
        {
            SetOptionHighlighted(3);
        }

        isOptionSelectingTouch = update.TouchpadPressState && currentOptionIndex != -1;
        if (!isOptionSelecting || currentOptionIndex != prevOptionIndex)
        {
            optionSelectPct = 0f;
        }
    }

    private void OnUpdate(float dt)
    {
        if (isOptionSelecting)
        {
            optionSelectPct += dt / SELECT_FILL_TIME;

            if (optionSelectPct >= 1f)
            {
                Service.UpdateManager.RemoveObserver(OnUpdate);
                SelectOption();
            }
        }
        SelectionFill.fillAmount = optionSelectPct;
    }

    private void SetOptionHighlighted(int optionIndex)
    {
        int numOptions = actionData.Options.Count;

        if (optionIndex >= 0 && optionIndex < numOptions)
        {
            currentOptionIndex = optionIndex;
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
        isOptionSelectingTrigger = update.TriggerPressState && currentOptionIndex != -1;
        if (!isOptionSelecting)
        {
            optionSelectPct = 0f;
        }
    }

    private void SelectOption()
    {
        actionData.OnOptionSelected(currentOptionIndex);
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
