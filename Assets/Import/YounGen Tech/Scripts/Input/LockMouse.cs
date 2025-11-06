using UnityEngine;
using System.Collections;

namespace YounGenTech {
	[AddComponentMenu("YounGen Tech/Scripts/Input/Lock Mouse")]
	public class LockMouse : MonoBehaviour {
		void Update() {
			if(Input.GetMouseButtonDown(2)) { Screen.lockCursor = !Screen.lockCursor; }
		}
	}
}