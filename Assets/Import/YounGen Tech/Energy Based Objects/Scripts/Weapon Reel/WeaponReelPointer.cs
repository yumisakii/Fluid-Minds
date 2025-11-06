using UnityEngine;

namespace YounGenTech.WeaponTech {

    /// <summary>
    /// Points the weapon wherever the mouse cursor hits with a Raycast
    /// </summary>
    [AddComponentMenu("YounGen Tech/Energy Based Objects/Weapon Reel/Pointer")]
    public class WeaponReelPointer : MonoBehaviour {
        public Quaternion rotation;
        public LayerMask pointAtMask = -1;
        public Camera lookCamera;

        void Awake() {
            rotation = transform.rotation;
        }

        void Update() {
            if(!lookCamera) return;

            Vector3 lookAt = transform.position + Vector3.forward;
            RaycastHit hit;

            if(Physics.Raycast(lookCamera.ScreenPointToRay(Input.mousePosition), out hit, Mathf.Infinity, -1))
                lookAt = hit.point;

            rotation = Quaternion.Slerp(rotation, Quaternion.LookRotation(lookAt - transform.position), Time.deltaTime * 20);
        }
    }
}