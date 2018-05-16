using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class LeaderController : NetworkBehaviour {

	public bool isLeader;

	public Transform ShotSpawnTransform;

	Transform mainCamera;

	public float cameraDistance = 16.0f;

	public float cameraHeight = 10.0f;

	Vector3 cameraOffset;

	public float MoveSpeed = 150.0f;

	public float RotateSpeed = 3.0f;

	public float ShotSpeed = 50.0f;

	public GameObject ShotPrefab;

	void Start(){
		mainCamera = Camera.main.transform;
		cameraOffset = new Vector3 (0, cameraHeight, -cameraDistance);
	}
	void Update () {
		if (!hasAuthority)
			return;

		MoveCamera ();

		var x = Input.GetAxis("Horizontal") * Time.deltaTime * MoveSpeed;
		var z = Input.GetAxis("Vertical") * Time.deltaTime * RotateSpeed;

		transform.Rotate(0, x, 0);
		transform.Translate(0, 0, z);

		if (Input.GetKeyDown (KeyCode.Space)) {
			CmdFire ();
		}
	}

	void MoveCamera(){
		mainCamera.position = transform.position;
		mainCamera.rotation = transform.rotation;
		mainCamera.Translate (cameraOffset);
		mainCamera.LookAt (transform);
	}

	[Command]
	void CmdFire(){
		var bullet = Instantiate (ShotPrefab, ShotSpawnTransform.position, Quaternion.identity) as GameObject;
		bullet.GetComponent<Rigidbody> ().velocity = transform.forward * ShotSpeed;

		NetworkServer.Spawn (bullet);
	}
}