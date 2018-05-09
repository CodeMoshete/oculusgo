using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CollisionTriggerZone : MonoBehaviour
{
	[System.Serializable]
	public struct TriggerParams
	{
		public GameObject Parent;
		public string AnimationName;
	}

	[System.Serializable]
	public struct TriggerCustomActionParams
	{
		public GameObject Parent;
	}

	public List<TriggerParams> Triggers;
	public List<TriggerCustomActionParams> Actions;

	// Use this for initialization
	void Start()
	{
	
	}
	
	// Update is called once per frame
	public void Update()
	{

	}

	public void OnTriggerEnter(Collider collisionObject) {
		print ("Collision Detected enter");
		if(collisionObject.tag == "Player")
		{
			for(int i = 0, count = Triggers.Count; i < count; i++)
			{
				Triggers[i].Parent.GetComponent<Animator>().SetTrigger(Triggers[i].AnimationName);
			}

			for(int i = 0, count = Actions.Count; i < count; i++)
			{
				Actions[i].Parent.GetComponent<CustomAction>().Initiate();
			}
		}
	}
}
