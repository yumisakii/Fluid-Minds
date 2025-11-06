using UnityEngine;
using System.Collections;

namespace YounGenTech.WeaponTech {

	/// <summary>
	/// A HUD for the GTB Machine Gun to show the <see cref="ObjectWeaponHeat"/> stats
	/// </summary>
	[AddComponentMenu("YounGen Tech/Energy Based Objects/Weapon Scripts/Machine Gun HUD")]
	public class MachineGunHeatHUD : MonoBehaviour {
		static GUIStyle centerBox;
		static GUIStyle centerLabel;

		ObjectWeaponHeat weaponHeat;

		void Awake() {
			weaponHeat = GetComponent<ObjectWeaponHeat>();

			if(!weaponHeat) Destroy(this);
		}

		void OnGUI() {
			if(centerBox == null) {
				centerBox = new GUIStyle(GUI.skin.box);
				centerBox.alignment = TextAnchor.UpperCenter;

				centerLabel = new GUIStyle(GUI.skin.label);
				centerLabel.alignment = TextAnchor.UpperCenter;
			}

			GUI.Box(new Rect(Screen.width * .5f - 150, Screen.height - 160, 300, 120), "", centerBox);
			GUI.Box(new Rect(Screen.width * .5f - 110, Screen.height - 160, 220, 120), "Heat: " + weaponHeat.heat.ToString("F1"), centerBox);

			GUI.Box(new Rect(Screen.width * .5f - 150, Screen.height - 130, 40, 30), "0");
			GUI.HorizontalSlider(new Rect(Screen.width * .5f - 100, Screen.height - 130, 200, 30), weaponHeat.heat, 0, weaponHeat.maxHeat);
			GUI.Box(new Rect(Screen.width * .5f + 110, Screen.height - 130, 40, 30), weaponHeat.maxHeat.ToString());

			weaponHeat.enabledShootingHeat = GUI.HorizontalSlider(new Rect(Screen.width * .5f - 100, Screen.height - 100, 200, 30), weaponHeat.enabledShootingHeat, 0, weaponHeat.maxHeat);
			GUI.Label(new Rect(Screen.width * .5f - 100, Screen.height - 80, 200, 30), "Enabled Shooting Heat: " + weaponHeat.enabledShootingHeat.ToString("F1"), centerLabel);
		}
	}
}