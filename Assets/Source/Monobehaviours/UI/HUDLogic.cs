﻿using UnityEngine;
using UnityEngine.UI;

public class HUDLogic : MonoBehaviour
{
    public GameObject TriggerPressContainer;

    public Text TextPromptContainer;
    public Animator TextPromptAnimator;

    public void Start()
    {
        Service.EventManager.AddListener(EventId.ShowTriggerPrompt, ShowTriggerPress);
        Service.EventManager.AddListener(EventId.ShowPromptText, ShowPromptText);
        Service.EventManager.AddListener(EventId.HidePromptText, HidePromptTextFromEvent);
    }

    public bool ShowTriggerPress(object cookie)
    {
        bool show = (bool)cookie;
        TriggerPressContainer.SetActive(show);
        return false;
    }

    public bool ShowPromptText(object cookie)
    {
        PromptTextActionData promptData = (PromptTextActionData)cookie;
        TextPromptContainer.text = promptData.Prompt;
        TextPromptAnimator.SetBool("IsVisible", true);
        if (promptData.Duration > 0)
        {
            Service.TimerManager.CreateTimer(promptData.Duration, HidePromptTextFromTimer, null);
        }
        return true;
    }

    public bool HidePromptTextFromEvent(object cookie)
    {
        TextPromptAnimator.SetBool("IsVisible", false);
        return true;
    }

    public void HidePromptTextFromTimer(object cookie)
    {
        TextPromptAnimator.SetBool("IsVisible", false);
    }
}
