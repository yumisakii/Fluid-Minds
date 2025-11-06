using UnityEngine;
using System.Collections.Generic;

namespace YounGenTech.WeaponTech {

    /// <summary>
    /// Handles setting the projectiles, changing the illumation material for the Gravity Gun.
    /// </summary>
    [AddComponentMenu("YounGen Tech/Energy Based Objects/Weapon Scripts/Gravity Gun/Gravity Gun")]
    public class GravityGunController : MonoBehaviour {

        ObjectPusher pusher;

        public Transform visualDistance;
        public float distance = 2;

        EnergyUser energyUser;
        Material gunMaterial;
        Color gunEmissionColor;
        float h, s, v;

        void Awake() {
            pusher = GetComponent<ObjectPusher>();
            energyUser = GetComponent<EnergyUser>();

            //Connect the gravity gun to its own energy source
            //It will change the Self-Illuminate material according to how much energy it has
            GetComponent<Energy>().OnModifiedEnergy += ChangeMaterial;

            gunMaterial = GetComponentInChildren<Renderer>().material;
            gunEmissionColor = gunMaterial.GetColor("_EmissionColor");
            Color.RGBToHSV(gunEmissionColor, out h, out s, out v);
        }

        void Update() {
            FindRigidbodyForProjectiles();

            visualDistance.localPosition = Vector3.forward * distance;
        }

        void ChangeMaterial(Energy energy, float amount) {
            float value = (energy.amount / energy.maxAmount) + 1;

            gunMaterial.SetColor("_EmissionColor", Color.HSVToRGB(h, s, value));
        }

        void OnGUI() {
            GUILayout.Box("Use the <b>Right Mouse Button</b> to see a full 360 of the Gravity Gun's model");
            pusher.useForceCurve = GUILayout.Toggle(pusher.useForceCurve, "<b>Use Force Curve (1 to -1, attracts halfway from the outer ring)</b> <color=red><i>*Turn off Implosion and slide Force up to see intended effect1*</i></color>");
            pusher.implosion = GUILayout.Toggle(pusher.implosion, "<b>Implosion</b>");

            GUILayout.BeginHorizontal();
            GUILayout.Box("<b>Force " + pusher.force.z.ToString("F1") + "</b>", GUILayout.Width(200));
            pusher.force.z = GUILayout.HorizontalSlider(pusher.force.z, 0, 1000, GUILayout.Width(200));
            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal();
            GUILayout.Box("<b>Distance from gun " + distance.ToString("F1") + "</b>", GUILayout.Width(200));
            distance = GUILayout.HorizontalSlider(distance, 0, 10, GUILayout.Width(200));
            GUILayout.EndHorizontal();
        }

        /// <summary>
        /// Finds Rigidbodies in the scene and sets them to be projectiles for the ObjectPusher
        /// </summary>
        public void FindRigidbodyForProjectiles() {
            pusher.worldPushPoint = transform.TransformPoint(Vector3.forward * distance);

            Collider[] colliders = Physics.OverlapSphere(transform.position, pusher.explosionRadius);
            List<GameObject> hitObjects = new List<GameObject>();

            foreach(Collider a in colliders)
                if(a.attachedRigidbody) {
                    if(Vector3.Dot(transform.forward, a.attachedRigidbody.position - transform.position) > 0) {
                        hitObjects.Remove(a.attachedRigidbody.gameObject);
                        hitObjects.Add(a.attachedRigidbody.gameObject);
                    }
                }

            /*------>    This is where the projectiles are being set    <------*/
            pusher.pushProjectiles = hitObjects.ToArray();
        }
    }
}