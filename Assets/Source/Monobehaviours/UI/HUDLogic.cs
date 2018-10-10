using UnityEngine;

public class HUDLogic : MonoBehaviour
{
    public GameObject TriggerPressContainer;

    public void Start()
    {
        Service.EventManager.AddListener(EventId.ShowTriggerPrompt, ShowTriggerPress);
    }

    public bool ShowTriggerPress(object cookie)
    {
        bool show = (bool)cookie;
        TriggerPressContainer.SetActive(show);
        return false;
    }
}
