using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AreaTrigger : MonoBehaviour
{
	[System.Serializable]
	public struct TriggerCustomActionParams
	{
		public GameObject Parent;
	}

	public List<TriggerCustomActionParams> EnterActions;
    public List<TriggerCustomActionParams> ExitActions;

    public void OnTriggerEnter(Collider collisionObject) {
		print ("Collision Detected enter");
		if(collisionObject.tag == "Player")
		{
			for(int i = 0, count = EnterActions.Count; i < count; i++)
			{
				EnterActions[i].Parent.GetComponent<CustomAction>().Initiate();
			}
		}
	}

    public void OnTriggerExit(Collider other)
    {
        print("Collision Detected exit");
        if (other.tag == "Player")
        {
            for (int i = 0, count = ExitActions.Count; i < count; i++)
            {
                ExitActions[i].Parent.GetComponent<CustomAction>().Initiate();
            }
        }
    }
}
