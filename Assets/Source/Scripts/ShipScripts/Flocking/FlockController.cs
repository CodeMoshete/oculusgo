using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class FlockController : MonoBehaviour {

	private const float MIN_COHESION_DIST = 20000.0f;
	private const float MAX_COHESION_DIST = 40000.0f;
	private const float AVOIDANCE_DIST = 30000.0f;
	private const float AIM_VARIATION = 150.0f;
	public int numToSpawn = 20;
	public int spawnAreaDims = 5;
	public float FlockScale = 1.0f;
	public List<string> possibleSpawns;
	public Vector3 spawnOrientation = new Vector3(0.0f, 0.0f, 0.0f);
	private int numInFlock;
	private ArrayList fullFlock;
	private Vector3 flockCenter;
	private GameObject flockTarget;
	private GameObject attackTarget;

	// Use this for initialization
	void Start () {
		attackTarget = GameObject.Find("BorgCube");

		possibleSpawns = new List<string>();
		possibleSpawns.Add("EnterpriseShipContainer");
		possibleSpawns.Add("SovereignShipContainer");
		possibleSpawns.Add("AkiraShipContainer");
		possibleSpawns.Add("SteamrunnerShipContainer");
		possibleSpawns.Add("DefiantShipContainer");
		possibleSpawns.Add("SaberShipContainer");
		possibleSpawns.Add("AmbassadorShipContainer");
		possibleSpawns.Add("NebulaShipContainer");

		int numPossibleSpawns = possibleSpawns.Count;

		fullFlock = new ArrayList();
		for(int i=0; i<numToSpawn; i++) 
		{
			Vector3 spawnPosition = new Vector3(transform.position.x + Random.Range(-spawnAreaDims,spawnAreaDims),
			                                    transform.position.y + Random.Range(-spawnAreaDims,spawnAreaDims), 
			                                    transform.position.z + Random.Range(-spawnAreaDims,spawnAreaDims));
			//Vector3 spawnPosition = new Vector3(transform.position.x - 5.0f, transform.position.y +5.0f, transform.position.z + 5.0f);
			//GameObject tmpObject = (GameObject) Instantiate(Resources.Load("EnterpriseShipContainer"), spawnPosition, Quaternion.Euler(spawnOrientation));
			string chosenSpawner = possibleSpawns[Random.Range(0,numPossibleSpawns)];
			GameObject tmpObject = (GameObject) Instantiate(Resources.Load(chosenSpawner), spawnPosition, Quaternion.Euler(spawnOrientation));
			//tmpObject.transform.parent = this.transform;
			tmpObject.transform.localScale = new Vector3(FlockScale, FlockScale, FlockScale);
			fullFlock.Add(tmpObject);
		}
		flockTarget = GameObject.Find("FlockTarget");
		numInFlock = numToSpawn;
	}
	
	// Update is called once per frame
	void Update () {
		updateFlockCenter();
		for(int i=0; i<numInFlock; i++)
		{
			processFlockerMovement((GameObject)fullFlock[i]);
			processFlockerAttack((GameObject)fullFlock[i]);
		}

		processFlockerAttack(attackTarget);
		attackTarget.transform.Rotate(new Vector3(0.0f,0.02f,0.0f));
	}

	void updateFlockCenter()
	{
		flockCenter = Vector3.zero;
		for(int i=0; i<numInFlock; i++)
		{
			flockCenter.x += ((GameObject)fullFlock[i]).transform.position.x;
			flockCenter.y += ((GameObject)fullFlock[i]).transform.position.y;
			flockCenter.z += ((GameObject)fullFlock[i]).transform.position.z;
		}

		flockCenter.x /= numInFlock;
		flockCenter.y /= numInFlock;
		flockCenter.z /= numInFlock;
	}

	void processFlockerMovement(GameObject flocker)
	{
		Vector3 avoidanceTest = getAvoidanceVector(flocker);
		Vector3 desiredVelocity = flocker.transform.forward;
		if(avoidanceTest != Vector3.zero)
		{
			print ("Avoiding collision!");
			desiredVelocity = avoidanceTest;
		}
		else
		{
			updateFlockerBehaviorWeights(flocker);

			float cohesionWeight = flocker.GetComponent<ShipFlocker>().cohesionWeight;
			float dispersionWeight = flocker.GetComponent<ShipFlocker>().dispersionWeight;
			float seekWeight = flocker.GetComponent<ShipFlocker>().seekWeight;

			float selection = Random.Range(0.0f, (cohesionWeight+dispersionWeight+seekWeight));
			//print("selection: " + selection + ", weight: " + cohesionWeight+dispersionWeight+seekWeight);

			if(selection < cohesionWeight)
			{
				//print ("cohesion");
				Vector3 cohesionComponent = applyCohesion(flocker);
				desiredVelocity = cohesionComponent;
				//cohesionComponent.Scale(new Vector3(cohesionScale,cohesionScale,cohesionScale));
			}
			else if(selection > cohesionWeight && selection < (cohesionWeight + dispersionWeight))
			{
				//print ("dispersion");
				Vector3 dispersionComponent = applyDispersion(flocker);
				desiredVelocity = dispersionComponent;
				//cohesionComponent.Scale(new Vector3(dispersionScale,dispersionScale,dispersionScale));
			}
			else if(selection > (cohesionWeight + dispersionWeight))
			{
				//print ("seek");
				if(flockTarget == null)
					flockTarget = this.gameObject;

				Vector3 seekRotation = applySeek(flocker, flockTarget.transform.position);
				desiredVelocity = seekRotation;
			}

			//desiredVelocity += seekComponent += cohesionComponent += dispersionComponent;
			//desiredVelocity = dispersionComponent;
		}

		flocker.GetComponent<ShipFlocker>().Rotate(desiredVelocity.x, desiredVelocity.y, desiredVelocity.z);
		flocker.transform.Translate(new Vector3(0.0f, 0.0f, flocker.GetComponent<ShipFlocker>().MaxSpeed));
	}

	void processFlockerAttack(GameObject flocker)
	{
		bool isBadGuy = flocker == attackTarget;
		GameObject target = (isBadGuy) ? (GameObject)fullFlock[Random.Range(0,numToSpawn-1)] : attackTarget;
		ShipFlocker flockComp = flocker.GetComponent<ShipFlocker>();
		bool shootPhasers = ((flockComp.PhaserShotsPerMinute/60.0f) * (Time.deltaTime)) > Random.Range(0.0f, 1.0f);
		if(shootPhasers)
		{
			//print ("Firing phasers!");
			GameObject closestPhaser = flockComp.phaserPoints[0];
			float closestDist = Mathf.Infinity;
			foreach(GameObject phsPt in flockComp.phaserPoints)
			{
				Vector3 distVec = phsPt.transform.position - target.transform.position;
				float sqrDst = Mathf.Pow(distVec.x,2) + Mathf.Pow(distVec.y,2) + Mathf.Pow(distVec.z,2);
				if(sqrDst < closestDist)
				{
					closestDist = sqrDst;
					closestPhaser = phsPt;
				}
			}
			string phaserToLoad = isBadGuy ? "BorgPhaser" : "FederationPhaser";
			GameObject phaserObj = (GameObject)Instantiate(Resources.Load(phaserToLoad),closestPhaser.transform.position,Quaternion.identity);
			phaserObj.transform.parent = flocker.transform;
			phaserObj.GetComponent<PhaserScript>().Launcher = flocker;
			phaserObj.GetComponent<PhaserScript>().setTarget(target.transform);
		}

		bool shootTorpedoes = ((flockComp.TorpedoShotsPerMinute/60.0f) * (Time.deltaTime)) > Random.Range(0.0f, 1.0f);
		if(shootTorpedoes)
		{
			GameObject closestTorpedo = flockComp.torpedoPoints[0];
			float closestDist = Mathf.Infinity;
			foreach(GameObject trpPt in flockComp.torpedoPoints)
			{
				Vector3 distVec = trpPt.transform.position - target.transform.position;
				float sqrDst = Mathf.Pow(distVec.x,2) + Mathf.Pow(distVec.y,2) + Mathf.Pow(distVec.z,2);
				if(sqrDst < closestDist)
				{
					closestDist = sqrDst;
					closestTorpedo = trpPt;
				}
			}
			string torpedoToLoad = isBadGuy ? "BorgTorpedo" : flockComp.TorpedoType;
			GameObject newTorp = (GameObject)Instantiate(Resources.Load(torpedoToLoad),closestTorpedo.transform.position,Quaternion.identity);
			TorpedoScript torp = newTorp.GetComponent<TorpedoScript>();
			Vector3 aimPos = new Vector3(target.transform.position.x + Random.Range(-AIM_VARIATION,AIM_VARIATION),
			                             target.transform.position.y + Random.Range(-AIM_VARIATION,AIM_VARIATION),
			                             target.transform.position.z + Random.Range(-AIM_VARIATION,AIM_VARIATION));
			Vector3 torpVel = (aimPos - closestTorpedo.transform.position).normalized;
			torpVel.Scale(new Vector3(torp.TORPEDO_SPEED,torp.TORPEDO_SPEED,torp.TORPEDO_SPEED));
			torp.setVelocity(torpVel);
			torp.Launcher = flocker;
		}
	}

	Vector3 applyCohesion(GameObject flocker)
	{	
		return applySeek(flocker, flockCenter);
	}

	Vector3 applyDispersion(GameObject flocker)
	{
		return applyFlee(flocker, flockCenter);
	}
	
	Vector3 applySeek(GameObject flocker, Vector3 target)
	{
		ShipFlocker flockComp = flocker.GetComponent<ShipFlocker>();
		float turnSpd = flockComp.TurnSpeed;
		Vector3 directionVector = flocker.transform.position - target;
		Vector3 orientation = flockComp.localEulerAngles;

		float yawDegrees = Mathf.DeltaAngle(orientation.y, (Mathf.Atan2(directionVector.x, directionVector.z) / (Mathf.PI/180))) + 180;
		float desiredYawRotation = flockComp.localEulerAngles.y;
		if(yawDegrees <= 180)
			desiredYawRotation = Mathf.Min(yawDegrees, turnSpd);

		else if(yawDegrees > 180)
			desiredYawRotation = Mathf.Max(-yawDegrees, -turnSpd);


		float desiredPitchRotation = 0;

		if(flocker.transform.position.y > target.y && ((flockComp.localEulerAngles.x > 180) || (flockComp.localEulerAngles.x < flockComp.MaxPitch)))
			desiredPitchRotation = turnSpd/3;
		else if(flocker.transform.position.y < target.y && (flockComp.localEulerAngles.x < 180) || (flockComp.localEulerAngles.x > (360.0f - flockComp.MaxPitch)))
			desiredPitchRotation = -turnSpd/3;

		Vector3 retVec = new Vector3(desiredPitchRotation, desiredYawRotation, 0.0f);

		return retVec;
	}

	Vector3 applyFlee(GameObject flocker, Vector3 target)
	{
		ShipFlocker flockComp = flocker.GetComponent<ShipFlocker>();
		float turnSpd = flockComp.TurnSpeed;
		Vector3 directionVector = flocker.transform.position - target;
		Vector3 orientation = flockComp.localEulerAngles;
		
		float yawDegrees = Mathf.DeltaAngle(orientation.y, (Mathf.Atan2(directionVector.x, directionVector.z) / (Mathf.PI/180))) + 180;
		float desiredYawRotation = flockComp.localEulerAngles.y;
		if(yawDegrees >= 180)
			desiredYawRotation = Mathf.Min(yawDegrees, turnSpd);
		
		else if(yawDegrees < 180)
			desiredYawRotation = Mathf.Max(-yawDegrees, -turnSpd);
		
		
		float desiredPitchRotation = 0;
		
		if(flocker.transform.position.y < target.y && ((flockComp.localEulerAngles.x > 180) || (flockComp.localEulerAngles.x < flockComp.MaxPitch)))
			desiredPitchRotation = turnSpd/3;
		else if(flocker.transform.position.y > target.y && (flockComp.localEulerAngles.x < 180) || (flockComp.localEulerAngles.x > (360.0f - flockComp.MaxPitch)))
			desiredPitchRotation = -turnSpd/3;
		
		Vector3 retVec = new Vector3(desiredPitchRotation, desiredYawRotation, 0.0f);
		
		return retVec;
	}

	Vector3 getAvoidanceVector(GameObject flocker)
	{
		if(attackTarget)
		{
			Vector3 fpos = flocker.transform.position;
			Vector3 bpos = attackTarget.transform.position;
			Vector3 distVec = fpos - bpos;
			float sqrDist = (Mathf.Pow(distVec.x,2) + Mathf.Pow(distVec.y,2) + Mathf.Pow(distVec.z,2));
			if(sqrDist <= AVOIDANCE_DIST)
				return applyFlee(flocker, attackTarget.transform.position);
	    }

		return Vector3.zero;
	}

	void updateFlockerBehaviorWeights(GameObject flocker)
	{
		Vector3 distVec = flocker.transform.position - flockCenter;
		float sqrDist = Mathf.Pow(distVec.x,2) + Mathf.Pow(distVec.y,2) + Mathf.Pow(distVec.z,2);

		//print(sqrDist);
		float cohesionWeight = Mathf.Min(Mathf.Max(sqrDist - MIN_COHESION_DIST, 0.0f) / (MAX_COHESION_DIST - MIN_COHESION_DIST), 1.0f);
		//print("cohesion: " + cohesionWeight + ", dispersion: " + (20-cohesionWeight));
		flocker.GetComponent<ShipFlocker>().cohesionWeight = cohesionWeight * 3;
		flocker.GetComponent<ShipFlocker>().dispersionWeight = 3-cohesionWeight;
	}
}