using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class ShotController : NetworkBehaviour {
	public float LifeTime = 2f;

	float age;

	// Use this for initialization
	void Start () {
		age = 0;
	}
		
	// Update is called once per frame
	[ServerCallback]
	void Update () {
		age += Time.deltaTime;
		if (age > LifeTime) {
			NetworkServer.Destroy (gameObject);
		}
	}

	void OnCollisionEnter(Collision other){
		if (!isServer)
			return;
		
		NetworkServer.Destroy (gameObject);
	}

}
