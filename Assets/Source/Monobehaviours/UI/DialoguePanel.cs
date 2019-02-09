using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DialoguePanel : MonoBehaviour
{
    public TextReveal DialogueText;
    public Image IconImage;

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
        gameObject.SetActive(true);
        DialogueText.text = (string)cookie;
        return true;
    }

    private void Update ()
    {
		
	}
}
