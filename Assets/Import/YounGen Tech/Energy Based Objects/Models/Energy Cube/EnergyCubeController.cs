using UnityEngine;
using System.Collections;

namespace YounGenTech.WeaponTech {
	[AddComponentMenu("YounGen Tech/Energy Based Objects/Weapon Reel/Energy Cube")]
	public class EnergyCubeController : MonoBehaviour {

		void Awake() {
			GetComponent<Energy>().OnModifiedEnergy += OnChangeMaterial;
		}

		void OnChangeMaterial(Energy energy, float delta) {
			StopAllCoroutines();
			StartCoroutine("ChangeMaterial", new Vector2(energy.amount, energy.maxAmount));
		}

		IEnumerator ChangeMaterial(Vector2 fromTo) {
			while(true) {
				float alpha = Mathf.MoveTowards(GetComponent<Renderer>().material.color.a, fromTo.x / fromTo.y, Time.deltaTime);
				GetComponent<Renderer>().material.color = new Color(1, 1, 1, alpha);

				if(alpha == fromTo.x / fromTo.y) yield break;
				yield return new WaitForEndOfFrame();
			}
		}
	}
}