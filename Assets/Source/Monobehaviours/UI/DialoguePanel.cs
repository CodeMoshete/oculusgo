using UnityEngine;
using UnityEngine.UI;

public class DialoguePanel : MonoBehaviour
{
    public TextReveal DialogueText;
    public Image IconImage;
    public Animator Animator;
    public GameObject PingEffect;

    private bool isTransitioning;

	private void Start ()
    {
        Service.EventManager.AddListener(EventId.ShowDialogueText, ShowDialogueText);
	}

    private void OnDestroy()
    {
        Service.EventManager.RemoveListener(EventId.ShowDialogueText, ShowDialogueText);
    }

    private bool ShowDialogueText(object cookie)
    {
        if (gameObject.activeSelf && !isTransitioning)
        {
            isTransitioning = true;
            gameObject.SetActive(true);
            Animator.SetBool("IsVisible", true);
            Service.TimerManager.CreateTimer(0.5f, TransitionInComplete, null);
        }
        TriggerPing();
        DialogueText.text = (string)cookie;
        return true;
    }

    private void TransitionInComplete(object cookie)
    {
        isTransitioning = false;
    }

    private bool HideDialogueText(object cookie)
    {
        if (gameObject.activeSelf && !isTransitioning)
        {
            isTransitioning = true;
            Animator.SetBool("IsVisible", false);
            Service.TimerManager.CreateTimer(0.5f, TransitionOutComplete, null);
        }
        return true;
    }

    private void TransitionOutComplete(object cookie)
    {
        isTransitioning = false;
        gameObject.SetActive(false);
    }

    private void TriggerPing()
    {
        PingEffect.SetActive(false);
        PingEffect.SetActive(true);
    }
}
