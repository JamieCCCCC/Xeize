using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

public class Health : NetworkBehaviour {
	public int MaxHealth;

	[SyncVar(hook ="OnHealthChanged")]
	public int CurrentHealth;
	public Text HealthScore;

	// Use this for initialization
	void Start () {
		HealthScore.text = CurrentHealth.ToString();
	}

	public void TakeDamage(int Howmuch){
		if (!isServer)
			return;
		var NewHealth = CurrentHealth - Howmuch;
		if (NewHealth <= 0) {
			RpcDeath ();
			CurrentHealth = MaxHealth;
		} else {
			CurrentHealth = NewHealth;
		}
	}

	public void OnHealthChanged(int updatedhealth){
		HealthScore.text = updatedhealth.ToString();
	}

	[ClientRpc]
	void RpcDeath(){
		if(isLocalPlayer){
//			var spawnpoints = FindObjectsOfType<NetworkStartPosition>();
//			int chosenpoint = Random.Range (0, spawnpoints.Length);
//			transform.position = spawnpoints [chosenpoint].transform.position;
			NetworkServer.Destroy(gameObject);
		}
	}
}
