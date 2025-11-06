using UnityEngine;
using System.Collections.Generic;

namespace YounGenTech.WeaponTech {

	/// <summary>
	/// Controls GameObjects with the Weapon tag. Weapons parented to this object will be shot from this component. This component will also control which Camera is active.
	/// </summary>
	[AddComponentMenu("YounGen Tech/Energy Based Objects/Weapon Reel/Shooter")]
	public class WeaponReelShooter : MonoBehaviour {
		public List<KeyValuePair<GameObject, Vector2>> guiWeaponInfo = new List<KeyValuePair<GameObject, Vector2>>();

		public GameObject selectedWeapon;
		public List<GameObject> weapons;
		public int number;
		
		WeaponReelPointer pointer;

		const float showControlsForTime = 3;
		float showControlsTime;

		void Awake() {
			pointer = GetComponent<WeaponReelPointer>();

			weapons.AddRange(GameObject.FindGameObjectsWithTag("Weapon"));
			weapons.Sort((x, y) => { return string.Compare(x.name, y.name); });

			for(int i = 0; i < weapons.Count; i++)
				if(i != 0) weapons[i].SetActive(false);

			selectedWeapon = weapons[0];
			number = 0;

			if(selectedWeapon) {
				CameraGroup cameraGroup = selectedWeapon.GetComponent<CameraGroup>();

				if(cameraGroup) {
					foreach(Camera a in FindObjectsOfType(typeof(Camera))) {
						GravityGunAudioVisual fps = a.GetComponent<GravityGunAudioVisual>();

						if(fps)
							fps.FPSActivate(false);
						else
							a.gameObject.SetActive(false);
					}

					GravityGunAudioVisual fpsCamera = cameraGroup.useCamera.GetComponent<GravityGunAudioVisual>();
					
					if(fpsCamera)
						fpsCamera.FPSActivate(true);
					else
						cameraGroup.ActivateCamera(true);
				}
			}
		}

		void Update() {
			if(Input.GetKeyDown(KeyCode.H)) showControlsTime = Time.time;
			if(Input.GetKeyDown(KeyCode.R) || Input.GetKeyDown(KeyCode.Joystick1Button7)) Application.LoadLevel(Application.loadedLevel);

			float scroll = Input.GetAxis("Mouse ScrollWheel");

			if(scroll != 0) {
				number += (int)Mathf.Sign(scroll);
				number %= weapons.Count;

				if(number < 0) number += weapons.Count;

				if(selectedWeapon) selectedWeapon.SetActive(false);

				selectedWeapon = weapons[number];
				selectedWeapon.SetActive(true);

				SwitchedTo(selectedWeapon);
			}

			if(selectedWeapon) {
				if(selectedWeapon.transform.IsChildOf(transform))
					selectedWeapon.transform.rotation = pointer.rotation;

				if(GUIUtility.hotControl == 0 && Input.GetButton("Fire1"))
					if(!selectedWeapon.GetComponent<CameraGroup>().useCamera.GetComponent<GravityGunAudioVisual>())
						selectedWeapon.GetComponentInChildren<ObjectShootingPoint>().Shoot();
			}
		}

		void OnGUI() {
			ControlsGUI();
			WeaponInfoGUI();
		}

		void SwitchedTo(GameObject weapon) {
			guiWeaponInfo.Add(new KeyValuePair<GameObject, Vector2>(weapon, new Vector2(0, 10)));

			if(weapon) {
				CameraGroup cameraGroup = weapon.GetComponent<CameraGroup>();

				if(cameraGroup) {
					foreach(Camera a in FindObjectsOfType(typeof(Camera))) {
						GravityGunAudioVisual fps = a.GetComponent<GravityGunAudioVisual>();

						if(fps)
							fps.FPSActivate(false);
						else
							a.gameObject.SetActive(false);
					}

					GravityGunAudioVisual fpsCamera = cameraGroup.useCamera.GetComponent<GravityGunAudioVisual>();

					if(fpsCamera)
						fpsCamera.FPSActivate(true);
					else
						cameraGroup.ActivateCamera(true);
				}
			}
		}

		void ControlsGUI() {
			float time = 1 - (Mathf.Clamp((Time.time - showControlsTime) - showControlsForTime, 0, showControlsForTime) / showControlsForTime);

			if(time > 0) {
				GUI.color = new Color(1, 1, 1, time);

				GUI.Box(new Rect(-15, Screen.height * .5f + 10, 180, 30), "<b>Left Mouse</b> - <b>Shoot</b>");
				GUI.Box(new Rect(-15, Screen.height * .5f + 50, 300, 30), "<b>Mouse Scroll Wheel</b> - <b>Change Weapon</b>");
				GUI.Box(new Rect(-15, Screen.height * .5f + 90, 160, 30), "<b>R</b> - <b>Restart level</b>");
			}
			else {
				GUI.Box(new Rect(-15, Screen.height * .5f + 10, 180, 30), "<b>H for Help</b>");
			}
		}

		void WeaponInfoGUI() {
			GUI.color = Color.white;
			List<KeyValuePair<GameObject, Vector2>> list = new List<KeyValuePair<GameObject, Vector2>>(guiWeaponInfo);

			if(weapons.Count > 1) {
				int leftIndex = (number - 1) % weapons.Count;
				if(leftIndex < 0) leftIndex += weapons.Count;

				int rightIndex = (number + 1) % weapons.Count;

				GUI.Box(new Rect(Screen.width * .5f - 450, Screen.height - 40, 300, 50), weapons[leftIndex].name + " <-");
				GUI.Box(new Rect(Screen.width * .5f + 150, Screen.height - 40, 300, 50), "-> " + weapons[rightIndex].name);
			}

			for(int i = 0; i < list.Count; i++) {
				guiWeaponInfo[i] = new KeyValuePair<GameObject, Vector2>(list[i].Key, new Vector2(list[i].Value.x + Time.deltaTime * (list.Count > 1 && i == 0 ? list[i].Value.y : 1), list[i].Value.y));

				GUI.color = Color.white;
				if(list[i].Value.x / list[i].Value.y > .5f)
					GUI.color *= 1 - ((list[i].Value.x / list[i].Value.y) - .5f);

				GUI.Box(new Rect(Screen.width * .5f - 150, (Screen.height - (Mathf.Clamp01(guiWeaponInfo[i].Value.x)) * 40), 300, 50), list[i].Key.name);
			}

			for(int i = 0; i < list.Count; i++)
				if(list[i].Value.x > list[i].Value.y) guiWeaponInfo.RemoveAt(i);
		}
	}
}