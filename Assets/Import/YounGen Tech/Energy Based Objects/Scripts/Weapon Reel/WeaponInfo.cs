using UnityEngine;
using System.Collections;

namespace YounGenTech.WeaponTech {

	/// <summary>
	/// Shortened and technical versions of the information about the weapon
	/// </summary>
	[AddComponentMenu("YounGen Tech/Energy Based Objects/Weapon Reel/Weapon Info")]
	public class WeaponInfo : MonoBehaviour {

		const float closedWidth = 60;
		const float openedWidth = 300;

		static bool technical;
		static bool open;
		static float width = closedWidth;

		[TextArea(false, false)]
		public string info = "";
		[TextArea(false, true)]
		public string technicalInfo = "";

		void OnGUI() {
			width = Mathf.MoveTowards(width, open ? openedWidth : closedWidth, Time.deltaTime * 300);

			Rect displayRect = new Rect(Screen.width - width, 100, width, Screen.height - 200);

			GUILayout.BeginArea(displayRect, GUI.skin.box);

			GUILayout.BeginHorizontal();
			GUILayout.FlexibleSpace();
			GUILayout.Label("Weapon Info");

			if(open && width >= openedWidth) {
				GUILayout.FlexibleSpace();
				technical = GUILayout.Toggle(technical, "Technical Info");
			}

			GUILayout.FlexibleSpace();
			GUILayout.EndHorizontal();

			GUILayout.BeginHorizontal();
			if(GUILayout.Button(open ? ">" : "<", GUILayout.ExpandWidth(false), GUILayout.ExpandHeight(true)))
				open = !open;


			if(open && width >= openedWidth) {
				GUILayout.BeginVertical();
				GUILayout.Label("<b>" + name + "</b>");

				if(technical)
					GUILayout.Label(technicalInfo);
				else {
					GUILayout.Label(info);
				}

				GUILayout.EndVertical();
			}

			GUILayout.EndHorizontal();
			GUILayout.EndArea();
		}
	}
}