using UnityEngine;

namespace YounGenTech.WeaponTech {

	/// <summary>
	/// Controls the audio and area-of-effect transform of the Gravity Gun. Also controls shooting.
	/// </summary>
	[AddComponentMenu("YounGen Tech/Energy Based Objects/Weapon Scripts/Gravity Gun/Audio Visual")]
	public class GravityGunAudioVisual : MonoBehaviour {

		public Transform areaOfEffect;
		public ObjectPusher pusher;

		bool modelPreview = false;

		public void FPSActivate(bool enable) {
			transform.parent.gameObject.SetActive(enable);
		}

		void OnEnable() { Screen.lockCursor = true; }
		void OnDisable() { Screen.lockCursor = false; }

		void Update() {
			if(GUIUtility.hotControl == 0 && Input.GetButton("Fire1")) {
				pusher.Shoot();

				//Change the pitch of the gravity gun as the energy goes down
				Energy energy = pusher.GetComponent<Energy>();
				EnergyUser user = pusher.GetComponent<EnergyUser>();

				if(energy.amount < user.useAmount)
					pusher.GetComponent<AudioSource>().pitch = Mathf.Lerp(pusher.GetComponent<AudioSource>().pitch, .6f, Time.deltaTime);
				else
					pusher.GetComponent<AudioSource>().pitch = Mathf.Lerp(.7f, .75f, energy.amount / energy.maxAmount);

				if(!pusher.GetComponent<AudioSource>().isPlaying)
					pusher.GetComponent<AudioSource>().Play();
			}
			else {
				if(pusher.GetComponent<AudioSource>().isPlaying)
					pusher.GetComponent<AudioSource>().Stop();
			}

			//Lock the cursor to the screen when you click
			if(GUIUtility.hotControl == 0 && Input.GetMouseButtonDown(0))
				if(!Screen.lockCursor) Screen.lockCursor = true;

			//Set the blue area of effect ring's size
			areaOfEffect.localScale = Vector3.one * pusher.explosionRadius * 2;

			Vector3 position = areaOfEffect.transform.position;
			position.x = pusher.worldPushPoint.x;
			position.z = pusher.worldPushPoint.z;
			position.y = .01f;
			areaOfEffect.transform.position = position;

			//360 Model Preview
			if(Input.GetMouseButtonDown(1))
				modelPreview = !modelPreview;

			if(modelPreview) {
				//pusher.transform.localPosition = new Vector3(0, 0, 1.5f);
				pusher.transform.Rotate(0, Time.deltaTime * 30, 0, Space.Self);
			}
			else {
				if(pusher.transform.localRotation != Quaternion.identity)
					pusher.transform.localRotation = Quaternion.identity;

				//if(pusher.transform.localPosition != pusherPosition)
				//	pusher.transform.localPosition = pusherPosition;
			}
		}
	}
}