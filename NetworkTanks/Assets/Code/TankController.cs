using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class TankController : NetworkBehaviour {
	void OnCollisionEnter(Collision collision){
		var other = collision.gameObject;
		if (other.CompareTag ("Projectile")) {
			try {
				var CauseDamageScript = other.GetComponent<CauseDamage> ();
				var TotalDamage = CauseDamageScript.GetDamage ();
				var HealthScript = GetComponent<Health> ();
				HealthScript.TakeDamage (TotalDamage);
			} catch {
				Debug.Log ("something hit me but didn't do a damage.");
			}
		}
	}
}