using UnityEngine;

namespace YounGenTech.WeaponTech {

	/// <summary>
	/// Controls the Rocket projectile
	/// </summary>
	[AddComponentMenu("YounGen Tech/Energy Based Objects/Weapon Scripts/Rocket")]
	public class RocketController : MonoBehaviour {

		TrailRenderer[] trails;

		void Awake() {
			trails = GetComponentsInChildren<TrailRenderer>();

			foreach(TrailRenderer trail in trails)
				trail.enabled = false;
		}

		void Update() {
			if(!GetComponent<Rigidbody>().isKinematic) {
				foreach(TrailRenderer trail in trails)
					trail.enabled = true;

				GetComponent<AudioSource>().Play();
				Destroy(this);
				Destroy(gameObject, 10);
			}
		}

	}
}