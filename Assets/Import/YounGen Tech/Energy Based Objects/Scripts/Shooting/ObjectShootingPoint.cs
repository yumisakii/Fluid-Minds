using UnityEngine;
using System;
using YounGenTech.ComponentInterface;
using UnityEngine.Events;

namespace YounGenTech.WeaponTech {

    [Serializable]
    public class ShootingPointEvent : UnityEvent<ObjectShootingPoint> { }

    /// <summary>
    /// The base script from which objects can shoot from
    /// </summary>
    [AddComponentMenu(WeaponTechHelper.ComponentMenuPath + "Shooting/Shooting Point")]
    public class ObjectShootingPoint : MonoBehaviour {

        public ShootingPointEvent OnShoot;
        public ShootingPointEvent OnLoad;

        /// <summary>
        /// Called when the weapon is shot
        /// Requires ObjectShootPoint as a parameter
        /// </summary>
        [Obsolete]
        public event Action<ObjectShootingPoint> OnWeaponShoot;

        /// <summary>
        /// Called when the weapon is loaded
        /// Requires ObjectShootPoint as a parameter
        /// </summary>
        [Obsolete]
        public event Action<ObjectShootingPoint> OnWeaponLoad;

        /// <summary>
        /// Called when the weapon is shot
        /// </summary>
        [Obsolete]
        public event Action OnWeaponShoot_NoParameters;

        /// <summary>
        /// Called when the weapon is loaded
        /// </summary>
        [Obsolete]
        public event Action OnWeaponLoad_NoParameters;

        /// <summary>
        /// <para>Acts as a conditional array of functions/delegates to check if this weapon can shoot.</para>
        /// <para>It is only checked when loading the weapon.</para>
        /// </summary>
        public event Func<bool> CanShoot;

        /// <summary>
        /// Automatically shoot
        /// </summary>
        public bool autoShoot;

        /// <summary>
        /// Automatically load when you shoot
        /// </summary>
        public bool autoLoadOnShoot = true;

        /// <summary>
        /// Automatically load
        /// </summary>
        public bool autoLoad;

        /// <summary>
        /// Delay between each shot
        /// </summary>
        public float shootDelay = .25f;

        /// <summary>
        /// Time left until you can shoot again
        /// </summary>
        public float shootTime;


        /// <summary>
        /// How many times you can load the weapon
        /// </summary>
        public int loadLimit = 1;

        /// <summary>
        /// How many times you have loaded the weapon
        /// </summary>
        public int loadCount;

        /// <summary>
        /// Delay between loading
        /// </summary>
        public float loadDelay;

        /// <summary>
        /// Time left until you can load again
        /// </summary>
        public float loadTime;

        /// <summary>
        /// Load this amount of times before allowing the weapon to shoot
        /// </summary>
        public int minLoadCountToShoot;

        /// <summary>
        /// If the loadTime is above zero, don't shoot
        /// </summary>
        public bool shootOnLoadTimerZero = true;

        /// <summary>
        /// Reset loadTime to zero when shot
        /// </summary>
        public bool resetLoadTimeOnShoot;

        #region Properties
        /// <summary>
        /// Is this weapon ready to Load
        /// </summary>
        public bool CanLoad {
            get { return IsShootTimerZero && IsLoadTimerZero && loadCount < loadLimit && CheckCanShootFunctions(); }
        }

        /// <summary>
        /// Is shootTime zero
        /// </summary>
        public bool IsShootTimerZero {
            get { return shootTime == 0; }
        }

        /// <summary>
        /// Is loadTime zero
        /// </summary>
        public bool IsLoadTimerZero {
            get { return loadTime == 0; }
        }

        /// <summary>
        /// Is loadCount greater or equal to minLoadCountToShoot
        /// </summary>
        public bool IsLoadCountAtMin {
            get { return loadCount >= minLoadCountToShoot; }
        }

        /// <summary>
        /// Has the weapon been loaded
        /// </summary>
        public bool ShotLoaded { get { return loadCount > 0 && IsLoadCountAtMin; } }
        #endregion

        /// <summary>
        /// Sends a message up through the hierarchy that it was enabled
        /// </summary>
        void OnEnable() {
            SendMessage("OnComponentWasEnabled", this, SendMessageOptions.DontRequireReceiver);
        }

        /// <summary>
        /// Sends a message up through the hierarchy that it was disabled
        /// </summary>
        void OnDisable() {
            SendMessage("OnComponentWasDisabled", this, SendMessageOptions.DontRequireReceiver);
        }

        void Update() {
            if(shootTime > 0) {
                shootTime -= Time.deltaTime;

                if(shootTime < 0) shootTime = 0;
            }

            if(loadTime > 0) {
                loadTime -= Time.deltaTime;

                if(loadTime < 0) loadTime = 0;
            }

            if(autoLoad)
                if(IsLoadTimerZero)
                    Load();

            if(autoShoot)
                if(IsShootTimerZero)
                    Shoot();
        }

        [ContextMenu("Shoot")]
        public virtual void Shoot() {
            if(!ShotLoaded && autoLoadOnShoot) Load();

            if(ShotLoaded && (shootOnLoadTimerZero ? IsLoadTimerZero : true)) {
                if(OnWeaponShoot_NoParameters != null) OnWeaponShoot_NoParameters();
                if(OnWeaponShoot != null) OnWeaponShoot(this);
                if(OnShoot != null) OnShoot.Invoke(this);

                if(resetLoadTimeOnShoot) loadTime = 0;

                shootTime = shootDelay;
                loadCount = 0;
            }
        }

        [ContextMenu("Load")]
        public virtual void Load() {
            if(CanLoad) {
                loadCount++;
                loadTime = loadDelay;

                if(OnWeaponLoad_NoParameters != null) OnWeaponLoad_NoParameters();
                if(OnWeaponLoad != null) OnWeaponLoad(this);
                if(OnLoad != null) OnLoad.Invoke(this);
            }
        }

        public bool CheckCanShootFunctions() {
            if(CanShoot == null) return true;

            foreach(Func<bool> shootingEvent in CanShoot.GetInvocationList()) {
                if(!shootingEvent()) return false;
            }

            return true;
        }

        public static Vector3 GetRandomDirection(float scale) {
            Vector3 random = Vector3.ClampMagnitude(new Vector3(1 - (UnityEngine.Random.value * 2), 1 - (UnityEngine.Random.value * 2), 1 - (UnityEngine.Random.value * 2)), 1);

            return random * scale;
        }

        public static Vector3 GetRandomDirection(Vector3 scale) {
            Vector3 random = Vector3.ClampMagnitude(new Vector3(1 - (UnityEngine.Random.value * 2), 1 - (UnityEngine.Random.value * 2), 1 - (UnityEngine.Random.value * 2)), 1);

            return Vector3.Scale(random, scale);
        }

        void OnComponentWasEnabled(Component component) {
            this.ConnectComponentEventTo(component);
        }

        void OnComponentWasDisabled(Component component) {
            this.DisconnectComponentEventFrom(component);
        }
    }
}