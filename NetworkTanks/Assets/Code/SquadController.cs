using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class SquadController : NetworkBehaviour {

	public GameObject Leader;

	public GameObject TankPrefab;

	public float spawnDistance = 5.0f;

	// Use this for initialization

	public override void OnStartLocalPlayer ()
	{
		CmdSpawnTank ();
	}
		
	[Command]
	void CmdSpawnTank(){
		Vector3 LeaderSP = transform.position;
		Leader = (GameObject)Instantiate(TankPrefab, LeaderSP,  Quaternion.identity);
		Leader.GetComponent<FollowerController>().enabled = false;
		Leader.GetComponent<TankController> ().enabled = true;
		NetworkServer.SpawnWithClientAuthority (Leader, connectionToClient);
		for(int i = 1; i < 3; i++ ) {
			Vector3 SP = new Vector3 (transform.position.x + spawnDistance*i, transform.position.y, transform.position.z + spawnDistance*i);
			GameObject Follower = (GameObject)Instantiate(TankPrefab, SP,  Quaternion.identity);
			Follower.GetComponent<TankController> ().enabled = false;
			Follower.GetComponent<FollowerController> ().enabled = true;
			NetworkServer.Spawn (Follower);
		}
	}

}
