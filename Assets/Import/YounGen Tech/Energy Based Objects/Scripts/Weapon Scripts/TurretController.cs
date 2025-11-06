using UnityEngine;
using UnityEngine.Serialization;
using System.Collections.Generic;

namespace YounGenTech.WeaponTech {

    /// <summary>
    /// Controls the Multi-Missile Turret
    /// </summary>
    [AddComponentMenu(WeaponTechHelper.ComponentMenuPath + "Weapon Scripts/Turret")]
    public class TurretController : MonoBehaviour {

        public const float ExtendedValue = .77f;
        public const float UnextendedValue = .54f;

        [SerializeField, FormerlySerializedAs("point")]
        ObjectShootingPoint _mainShootingPoint;

        [SerializeField, FormerlySerializedAs("crosshair")]
        Transform _crosshair;

        [SerializeField, FormerlySerializedAs("turnAudio")]
        AudioSource _turnAudio;

        [SerializeField]
        Transform _yaw;

        [SerializeField]
        Transform _pitch;

        [SerializeField]
        AudioSource _yawAudio;

        [SerializeField]
        AudioSource _pitchAudio;

        GameObject[] heads;
        ObjectShootingPoint[] shootingPoints = new ObjectShootingPoint[0];
        Vector2 input;

        #region Properties
        public Transform Crosshair {
            get { return _crosshair; }
            set { _crosshair = value; }
        }

        public GameObject[] Heads {
            get { return heads; }
            private set { heads = value; }
        }

        public ObjectShootingPoint MainShootingPoint {
            get { return _mainShootingPoint; }
            set { _mainShootingPoint = value; }
        }

        public Transform Pitch {
            get { return _pitch; }
            set { _pitch = value; }
        }

        public AudioSource PitchAudio {
            get { return _pitchAudio; }
            set { _pitchAudio = value; }
        }

        public AudioSource TurnAudio {
            get { return _turnAudio; }
            set { _turnAudio = value; }
        }

        public Transform Yaw {
            get { return _yaw; }
            set { _yaw = value; }
        }

        public AudioSource YawAudio {
            get { return _yawAudio; }
            set { _yawAudio = value; }
        }
        #endregion

        void Awake() {
            MainShootingPoint.OnLoad.AddListener(OnLoad);

            SetupTurretHeads();
            StartupHeadPositions();

            Yaw = transform.Find("PivotYaw");
            Pitch = transform.Find("PivotYaw/PivotPitch");
        }

        void Update() {
            if(GUIUtility.hotControl == 0 && Input.GetButtonDown("Fire1")) {
                if(MainShootingPoint.loadLimit == 0)
                    SetupTurretHeads();

                MainShootingPoint.Shoot();
            }

            input.x = Input.GetAxis("Horizontal");
            input.y = Input.GetAxis("Vertical");

            UpdateHeadRotations();
            UpdateHeadPositions();
            UpdateCrosshair();
        }

        void OnGUI() {
            GUILayout.Box("<b>Left Mouse/A(controller)</b> to <b>Fire</b>");
            GUILayout.Box("<b>WASD/Arrows/Left Joystick(controller)</b> to <b>Rotate</b>");
            GUILayout.Box("<b>R/Start(controller)</b> to <b>reset</b>");
        }

        /// <summary>
        /// Connect the child <see cref="ObjectShootingPoint"/>s so when this <see cref="ObjectShootingPoint"/> fires 
        /// the children will too
        /// </summary>
        void ConnectShootingPoints() {
            List<ObjectShootingPoint> list = new List<ObjectShootingPoint>();

            foreach(GameObject a in Heads)
                foreach(ObjectShootingPoint childPoint in a.GetComponentsInChildren<ObjectShootingPoint>()) {
                    MainShootingPoint.OnShoot.AddListener(s => childPoint.Shoot());
                    list.Add(childPoint);
                }

            shootingPoints = list.ToArray();
        }

        /// <summary>
        /// Sets the minimum amount load projectiles to load based on the amount of child <see cref="ObjectShootingPoint"/>s that there are.
        /// </summary>
        void SetupTurretHeads() {
            List<GameObject> headList = new List<GameObject>();

            foreach(ObjectShootingPoint a in GetComponentsInChildren<ObjectShootingPoint>(false)) {
                if(a.transform == transform) continue;

                headList.Add(a.transform.parent.gameObject);
            }

            Heads = headList.ToArray();

            ConnectShootingPoints();

            MainShootingPoint.loadLimit = shootingPoints.Length;
            MainShootingPoint.minLoadCountToShoot = shootingPoints.Length;
        }

        /// <summary>
        /// Loads the <see cref="ObjectShootingPoint"/> of a head based on the load count of the main shooting point.
        /// </summary>
        void OnLoad(ObjectShootingPoint shootingPoint) {
            shootingPoints[shootingPoint.loadCount - 1].Load();
        }

        void UpdateCrosshair() {
            if(Crosshair) {
                RaycastHit hit;
                Vector3 position = Pitch.TransformPoint(Vector3.down * 10);

                if(Physics.Raycast(Pitch.position, Quaternion.Euler(1f, 0, 0) * -Pitch.up, out hit, Mathf.Infinity))
                    position = hit.point;

                Crosshair.position = position;
            }
        }

        void StartupHeadPositions() {
            foreach(GameObject head in Heads) {
                ObjectShootingPoint shootingPoint = head.GetComponentInChildren<ObjectShootingPoint>();

                if(shootingPoint) {
                    Vector3 targetPosition = (shootingPoint.IsLoadCountAtMin && MainShootingPoint.IsShootTimerZero) ? Vector3.up * ExtendedValue : Vector3.up * UnextendedValue;

                    head.transform.localPosition = targetPosition;
                }
            }
        }

        void UpdateHeadPositions() {
            foreach(GameObject head in Heads) {
                ObjectShootingPoint shootingPoint = head.GetComponentInChildren<ObjectShootingPoint>();

                if(shootingPoint) {
                    Vector3 targetPosition = (shootingPoint.IsLoadCountAtMin && MainShootingPoint.IsShootTimerZero) ? Vector3.up * ExtendedValue : Vector3.up * UnextendedValue;

                    head.transform.localPosition = Vector3.MoveTowards(head.transform.localPosition, targetPosition, Time.deltaTime);
                }
            }
        }

        void UpdateHeadRotations() {
            if(input.x != 0) {
                Yaw.Rotate(Vector3.forward * Time.deltaTime * input.x * 5);

                if(!YawAudio.isPlaying) {
                    YawAudio.Play();
                    TurnAudio.Play();
                }

                YawAudio.pitch = .8f + Mathf.Abs(input.x) * .29f;
            }
            else if(YawAudio.isPlaying) {
                YawAudio.Stop();
                TurnAudio.Play();
            }

            if(input.y != 0) {
                Pitch.Rotate(Vector3.right * Time.deltaTime * input.y * 5);

                if(!PitchAudio.isPlaying) {
                    PitchAudio.Play();
                    TurnAudio.Play();
                }

                PitchAudio.pitch = .8f + Mathf.Abs(input.y) * .29f;
            }
            else if(PitchAudio.isPlaying) {
                PitchAudio.Stop();
                TurnAudio.Play();
            }
        }
    }
}