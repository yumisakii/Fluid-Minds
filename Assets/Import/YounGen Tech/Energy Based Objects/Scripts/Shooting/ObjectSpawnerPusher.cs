using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

namespace YounGenTech.WeaponTech {

    /// <summary>
    /// Spawns projectiles then pushes them
    /// </summary>
    [AddComponentMenu(WeaponTechHelper.ComponentMenuPath + "Shooting/Spawner Pusher")]
    public class ObjectSpawnerPusher : ObjectPusher {

        /// <summary>
        /// If set, it will use this function instead to add a projectile to the list. 
        /// Useful for if you have your own Object Pool. 
        /// </summary>
        public Func<GameObject> OverrideSpawnThisObject;

        /// <summary>
        /// The object to instantiate and push
        /// </summary>
        public GameObject spawnThisObject;

        /// <summary>
        /// How many objects should be instantiated
        /// </summary>
        public int spawnAmount = 1;

        /// <summary>
        /// If you are spawning a rigidbody, set isKinematic to true;
        /// </summary>
        public bool isKinematicOnSpawn;

        /// <summary>
        /// Parent the spawned object to this object;
        /// </summary>
        public bool parentOnSpawn;
        /// <summary>
        /// If parentOnSpawn is true, this object will be the parent.
        /// Parents to self if parentTo is not set
        /// </summary>
        public Transform parentTo;

        void Awake() {
            OnLoad.AddListener(s => Instantiate());

            OnShoot.AddListener(s => Push());
            OnShoot.AddListener(s => ClearProjectiles());
        }

        public void Instantiate() {
            List<GameObject> list = new List<GameObject>(pushProjectiles);

            for(int i = 0; i < spawnAmount; i++) {
                GameObject go;

                if(OverrideSpawnThisObject != null) {
                    go = OverrideSpawnThisObject();

                    if(!go) {
#if UNITY_EDITOR
                        Debug.LogError(typeof(ObjectSpawnerPusher).Name + " - Null returned from OverrideSpawnThisObject.");
#endif
                        continue; //Maybe OverrideSpawnThisObject will return not Null on the next loop?
                    }
                }
                else {
                    if(!spawnThisObject) break;

                    go = Instantiate(spawnThisObject, transform.position, transform.rotation) as GameObject;
                }

                if(go) {
                    go.transform.position = transform.position;
                    go.transform.rotation = transform.rotation;

                    if(isKinematicOnSpawn && go.GetComponent<Rigidbody>())
                        go.GetComponent<Rigidbody>().isKinematic = true;

                    if(parentOnSpawn)
                        go.transform.parent = parentTo ? parentTo : transform;

                    list.Add(go);
                }
            }

            pushProjectiles = list.ToArray();
        }
    }
}