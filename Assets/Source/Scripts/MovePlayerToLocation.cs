using UnityEngine;
using System.Collections;

public class MovePlayerToLocation : CustomAction {

	public GameObject Player;
	public GameObject TargetPosition;
	public float InitialVelocity;
	public float Acceleration;

	private bool isActive;

	public override void Initiate()
	{
		isActive = true;
		Player.GetComponent<CharacterController>().enabled = false;
	}
	
	// Update is called once per frame
	void Update () {
		if(isActive)
		{
			InitialVelocity += Acceleration;
			Vector3 vectorToTarget = TargetPosition.transform.position - Player.transform.position;

			if(vectorToTarget.magnitude < InitialVelocity)
			{
				Player.transform.position = TargetPosition.transform.position;
				isActive = false;
				InitialVelocity = 0f;
			}
			else
			{
				Vector3 nextPos = (vectorToTarget).normalized * InitialVelocity;
				Vector3 currentPos = Player.transform.position;
				Player.transform.position = currentPos + nextPos;
			}
		}
	}
}
