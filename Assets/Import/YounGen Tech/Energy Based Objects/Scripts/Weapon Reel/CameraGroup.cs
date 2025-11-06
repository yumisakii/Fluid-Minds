using UnityEngine;

namespace YounGenTech.WeaponTech {

	/// <summary>
	/// The camera that will be used by a weapon in the WeaponReel scene
	/// </summary>
	[AddComponentMenu("YounGen Tech/Energy Based Objects/Weapon Reel/Camera Group")]
	public class CameraGroup : MonoBehaviour {

		public Camera useCamera;

		public void ActivateCamera(bool enable) {
			useCamera.gameObject.SetActive(enable);
		}
	}
}