using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class GuardTowerController : NetworkBehaviour {
	
	public enum AISTATE {IDLE=0,ATTACK=1};
	public AISTATE CurrentState = AISTATE.IDLE;
	private Transform PlatformTransform = null;
	private Transform GunTransform = null;
	private List<Transform> TargetObjects = new List<Transform>();
	private Transform target;
	private float NextTargetTime;
	private float NextShotTime;

	//AI Visiility Settings
	public bool CanSeePlayer = false;
	public GameObject ShotPrefab;
	public float ViewAngle = 90f;
	public float AttackDistance = 1f;
	public float RotateSpeed = 1.0f;
	public float ShotSpeed = 50.0f;
	public float RetargetRate = 2.0f;
	public float ShotRate = 0.5f;

	public GameObject platform;
	public GameObject pointer;
	public Transform ShotSpawnTransform;
	//-----------------------------------
	// Use this for initialization
	void Awake () 
	{
		PlatformTransform = platform.GetComponent<Transform>();
		GunTransform = pointer.GetComponent<Transform>();
	}
	//-----------------------------------
	void Start()
	{
		//Set Starting State
		ChangeState (CurrentState);
	}
	//-----------------------------------
	void Update(){
		if (CurrentState == AISTATE.ATTACK) {
			Vector3 platformdirection = target.position-PlatformTransform.position;
			platformdirection.y = 0;
			PlatformTransform.rotation = Quaternion.LookRotation(platformdirection);

			Vector3 gundirection = target.position-GunTransform.position;
			Quaternion gunrotation = Quaternion.FromToRotation (Vector3.up, gundirection);
			GunTransform.rotation = gunrotation;
		}
	}
	public IEnumerator Idle()
	{
		//Loop while idling
		while(CurrentState == AISTATE.IDLE)
		{
			//Deal Idle here
			PlatformTransform.Rotate(0,RotateSpeed,0);
			if(CanSeePlayer)
			{
				ChangeState (AISTATE.ATTACK);
				yield break;
			}

			yield return null;
		}
	}
	//-----------------------------------
	public IEnumerator Attack()
	{
		while(CurrentState == AISTATE.ATTACK)
		{
			//Deal damage here
			if(Time.time > NextTargetTime){
				if (isServer) {
					int chosentarget = Random.Range (0, TargetObjects.Count);
					RpcUpdateChosenTarget (chosentarget);
				}
				NextTargetTime = Time.time + RetargetRate;
			}
			if (Time.time > NextShotTime) {
				CmdFire ();
				NextShotTime = Time.time + ShotRate;
			}
			if(!CanSeePlayer)
			{
//				Vector3.Distance (GunTransform.position, TargetObject.position) > AttackDistance
				ChangeState (AISTATE.IDLE);
			}

			yield return null;
		}
	}
	//-----------------------------------
	public void ChangeState(AISTATE NewState)
	{
		StopAllCoroutines ();
		CurrentState = NewState;

		switch(NewState)
		{
		case AISTATE.IDLE:
			StartCoroutine (Idle());
			break;
		case AISTATE.ATTACK:
			StartCoroutine (Attack());
			break;
		}
	}
	//-----------------------------------
	void OnTriggerStay(Collider Col)
	{
		if(!Col.CompareTag ("Player"))
			return;

		//Player transform
		Transform PlayerTransform = Col.GetComponent<Transform>();

		if (TargetObjects.Contains (PlayerTransform))
			return;

		Debug.Log ("Start to check enter");
		if (CurrentState != AISTATE.ATTACK) {
			//Is player in sight
			Vector3 DirToPlayer = PlayerTransform.position - GunTransform.position;
			Vector3 v1 = DirToPlayer;
			Vector3 v2 = GunTransform.forward;
			v1.y = 0.0f;
			v2.y = 0.0f;

			//Get viewing angle
			float ViewingAngle = Mathf.Abs (Vector3.Angle (v2, v1));
			if (ViewingAngle > ViewAngle)
				return;
		}
		
		TargetObjects.Add (PlayerTransform);
		CanSeePlayer = true;
		Debug.Log ("added");
	}
	//-----------------------------------
	void OnTriggerExit(Collider Col)
	{
		if(!Col.CompareTag ("Player"))
			return;

		Transform PlayerTransform = Col.GetComponent<Transform>();

		if (!TargetObjects.Contains (PlayerTransform))
			return;
		
		TargetObjects.Remove (PlayerTransform);
		Debug.Log ("removed");
		if(TargetObjects.Count.Equals (0))
			CanSeePlayer = false;
	}
	[ClientRpc]
	void RpcUpdateChosenTarget(int chosentarget){
		target = TargetObjects [chosentarget];
	}
	[Command]
	void CmdFire(){
		var bullet = Instantiate (ShotPrefab, ShotSpawnTransform.position, Quaternion.identity) as GameObject;
		bullet.GetComponent<Rigidbody> ().velocity = GunTransform.up * ShotSpeed;

		NetworkServer.Spawn (bullet);
	}
}