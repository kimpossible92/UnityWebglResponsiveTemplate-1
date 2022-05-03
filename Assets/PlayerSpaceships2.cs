using System.Collections.Generic;
using NWH.NPhysics;
using NWH.VehiclePhysics2.Effects;
using NWH.VehiclePhysics2.Input;
using NWH.VehiclePhysics2.Modules;
using NWH.VehiclePhysics2.Powertrain;
using NWH.VehiclePhysics2.Powertrain.Wheel;
using NWH.VehiclePhysics2.Sound;
using NWH.WheelController3D;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;
using UnityEngine.InputSystem;
#if UNITY_EDITOR
using UnityEditor;

#endif
namespace NWH.VehiclePhysics2
{
    [RequireComponent(typeof(NRigidbody))]
    public class PlayerSpaceships2 : NWH.Vehicle
    {
        public EffectManager effectsManager = new EffectManager();
        public SoundManager soundManager = new SoundManager();
        public Steering steering = new Steering();
        public Powertrain.Powertrain powertrain = new Powertrain.Powertrain();
        public float wheelbase = 4f; public NRigidbody vehicleNRigidbody;
        public float fixedDeltaTime = 0.02f;
        bool MouseHeel = false;
        float pointrot = 0f; float pointrot2 = 0;
        Vector2 _moveDirection;
        public float Speed2 = 10f;
        public float JumpForce = 300f;
        private bool _isGrounded;
        private Rigidbody _rb;
        [SerializeField]
        private float _lateralMovementSpeed;
        public Vector3 inertiaTensor = new Vector3(370f, 1640f, 1150f);
        [SerializeField]
        private float _longitudinalMovementSpeed;
        [SerializeField] LayerMask _layerMask; public static float timeSpent;
        private bool freezeWhileIdle;
        private float _sleepTimer;

        public List<WheelGroup> WheelGroups
        {
            get { return powertrain.wheelGroups; }
            private set { powertrain.wheelGroups = value; }
        }
        public List<WheelComponent> Wheels
        {
            get { return powertrain.wheels; }
            private set { powertrain.wheels = value; }
        }
        [Range(1, 30)]
        [Tooltip(
   "    Number of substeps when vehicle speed is\r\n    < 1m/ s.\r\n        Larger number reduces creep but decreases performance.")]
        public int lowSpeedSubsteps = 25;

        [Range(1, 30)]
        [Tooltip("    Number of physics substeps when vehicle speed is >= 1m/s.")]
        public int highSpeedSubsteps = 20;
        [Range(1, 20)]
        [Tooltip("    Number of physics substeps when vehicle is asleep.")]
        public int asleepSubsteps = 2;
        private float drag;
        private float mass = 1400f;
        private float angularDrag;
        private Vector3 centerOfMass = Vector3.zero;
        private RigidbodyInterpolation interpolation;
        private RigidbodyConstraints _initialRbConstraints;
        private float maxAngularVelocity = 8f;
        public void RotZero()
        {
            pointrot = 0f;
            pointrot2 = 0f;
        }
        public void OnMove(InputAction.CallbackContext context)
        {

            float hMove = context.ReadValue<Vector2>().x;
            float vMove = context.ReadValue<Vector2>().y;
            if (vMove < 0 || vMove > 0)
            {
                timeSpent += Time.deltaTime;
                if (timeSpent < 3f)
                {
                    float remainder = timeSpent % .8f;
                    vMove = 0.8f;
                }
                else
                {
                    //isVMove = false;
                }

            }
            if (vMove == 0) { vMove = 0.3f; }
            _moveDirection = new Vector2(hMove, vMove * 2f);
        }

        // Use this for initialization
        void Start()
        {
            ApplyInitialRigidbodyValues();
            vehicleNRigidbody.OnPrePhysicsSubstep += OnPrePhysicsSubstep;
            vehicleNRigidbody.OnPhysicsSubstep += OnPhysicsSubstep;
            vehicleNRigidbody.OnPostPhysicsSubstep += OnPostPhysicsSubstep;
            if (powertrain.wheelGroups.Count == 2
                            && powertrain.wheelGroups[0].Wheels.Count == 2
                            && powertrain.wheelGroups[1].Wheels.Count == 2)
            {
                wheelbase = Vector3.Distance(
                    powertrain.wheelGroups[0].LeftWheel.wheelController.transform.position,
                    powertrain.wheelGroups[1].LeftWheel.wheelController.transform.position);
            }
        }
        public virtual void OnPhysicsSubstep(float t, float dt, int i)
        {
            if (_multiplayerInstanceType == MultiplayerInstanceType.Local)
            {
                powertrain.OnPhysicsSubstep(t, dt, i);
            }
        }
        public virtual void OnPostPhysicsSubstep(float t, float dt)
        {
            if (_multiplayerInstanceType == MultiplayerInstanceType.Local)
            {
                powertrain.OnPostPhysicsSubstep(t, dt);
                vehicleNRigidbody.Substeps =
                    isAwake ? Speed < 2 ? lowSpeedSubsteps : highSpeedSubsteps : asleepSubsteps;
            }
            else
            {
                vehicleNRigidbody.Substeps = 1;
            }
        }
        public virtual void OnPrePhysicsSubstep(float t, float dt)
        {
            fixedDeltaTime = dt; ApplyLowSpeedFixes();
        }
        private void ApplyLowSpeedFixes()
        {
            // Increase inertia when still to mitigate jitter at low dt.
            float angVelSqrMag = vehicleRigidbody.angularVelocity.sqrMagnitude;
            float t = VelocityMagnitude * 0.25f + angVelSqrMag * 1.1f;
            float inertiaScale = Mathf.Lerp(4f, 1f, t);
            vehicleRigidbody.inertiaTensor = inertiaTensor * inertiaScale;

            // Freeze while idle
            if (freezeWhileIdle)
            {
                float verticalInput = _moveDirection.y;
                float absVertInput = verticalInput < 0 ? -verticalInput : verticalInput;
                float d = absVertInput * 2f + vehicleRigidbody.velocity.magnitude * 4f + vehicleRigidbody.angularVelocity.magnitude * 4f;

                if (d < 1f)
                {
                    vehicleRigidbody.constraints =
                        RigidbodyConstraints.FreezePositionX | RigidbodyConstraints.FreezePositionZ;
                }
                else
                {
                    vehicleRigidbody.constraints = RigidbodyConstraints.None;
                }
            }

            _sleepTimer += fixedDeltaTime;
        }

        // Update is called once per frame
        void Update()
        {
            effectsManager.Update(); soundManager.Update();
            if (GetComponent<Rigidbody>().isKinematic == true || GetComponent<CollShip>()._pause) { return; }
            if (!MMDebug.Raycast3DBoolean(transform.position, new Vector3(0, -1, 0), 2.5f, _layerMask, Color.red, true))
            {
                LongMovement(0.8f);
                return;
            }
            if (GetComponent<CollShip>()._pause || GetComponent<CollShip>().IsMenu)
            {
                _moveDirection = Vector3.zero;
                return;
            }
            _rb = GetComponent<Rigidbody>();
            pointrot += _moveDirection.x * Time.deltaTime * -100;
            LateralRotate(pointrot);
            LongMovement(_moveDirection.y);

        }
        private void OnDrawGizmosSelected()
        {

        }
        public void LateralRotate(float amount)
        {
            transform.rotation = Quaternion.Euler(0, -amount, 0);
        }
        public void LongMovement(float amount)
        {
            Move(amount * 10, Vector3.forward);
        }
        private void Move(float amount, Vector3 axis)
        {
            transform.Translate(-amount * axis * Time.deltaTime);
        }
        public virtual void Reset()
        {
            SetDefaults();
        }
        public virtual void SetDefaults()
        {
#if NVP2_DEBUG
            Debug.Log($"SetDefaults() [{name}]");
#endif

            ApplyInitialRigidbodyValues();
        }
        public virtual void ApplyInitialRigidbodyValues()
        {
            if (vehicleRigidbody == null)
            {
                vehicleRigidbody = GetComponent<Rigidbody>();
                Debug.Assert(vehicleRigidbody != null, "Vehicle does not have a Rigidbody.");
            }

            // Apply initial rigidbody values
            vehicleRigidbody.interpolation = RigidbodyInterpolation.Interpolate;
            vehicleRigidbody.maxAngularVelocity = maxAngularVelocity;
            vehicleRigidbody.drag = drag;
            vehicleRigidbody.mass = mass;
            vehicleRigidbody.angularDrag = angularDrag;
            vehicleRigidbody.centerOfMass = centerOfMass;
            vehicleRigidbody.inertiaTensor = inertiaTensor;
            vehicleRigidbody.sleepThreshold = 0;
            vehicleRigidbody.interpolation = interpolation;
            _initialRbConstraints = vehicleRigidbody.constraints;
        }
    }
}