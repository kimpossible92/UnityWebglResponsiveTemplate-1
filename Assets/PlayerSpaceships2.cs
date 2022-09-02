using System.Collections.Generic;

using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;
using UnityEngine.InputSystem;
#if UNITY_EDITOR
using UnityEditor;

#endif
public class PlayerSpaceships2 : MonoBehaviour
{
    //public EffectManager effectsManager = new EffectManager();
    //public SoundManager soundManager = new SoundManager();
    //public Steering steering = new Steering();
    //public Powertrain.Powertrain powertrain = new Powertrain.Powertrain();
    #region Airplane_Movment
    public CharacterController controller;

    public float airplaneSpeed;
    public float maxSpeed = 180f;
    public float minSpeed = 60f;
    public float speedAcceleration = 3f;
    public float rotateSpeed = 20f;
    private float boostValue;
    private bool boost;
    public float minRollAngle = -45f, maxRollAngle = 45f;
    private float yaw = 0f, pitch = 0f, roll = 0f;
    public float resetRotationSpeed = 100f;


    private void applayForwardMovement() {
        Vector3 direction = transform.forward.normalized;
        airplaneSpeed -= transform.forward.y * speedAcceleration * Time.deltaTime;
        airplaneSpeed = Mathf.Clamp(airplaneSpeed, minSpeed, maxSpeed);
        controller.Move(direction * (airplaneSpeed + (airplaneSpeed > 0f  ? boostValue : 0f)) * Time.deltaTime);
    }
    void FixedUpdate() {
        applayForwardMovement();
    }
    public void OnMoved(InputAction.CallbackContext context)
    {
        float horizontal = context.ReadValue<Vector2>().x;
        float vertical = context.ReadValue<Vector2>().y;
        pitch += vertical * rotateSpeed * Time.deltaTime;
        yaw += horizontal * rotateSpeed * Time.deltaTime;
        roll += horizontal;
        if (horizontal == 0.0f) {
            float reset = resetRotationSpeed * Time.deltaTime;
            roll = roll > 0f ? roll - Mathf.Min(roll, reset) : roll + Mathf.Min(-roll, reset);
        }
        roll = Mathf.Clamp(roll, minRollAngle, maxRollAngle);
        transform.rotation = Quaternion.Euler(new Vector3(pitch, yaw, -roll));
    }
    public void handleBoost(InputAction.CallbackContext context)
    {
        if (context.action.name == "E")
        {
            print("boost");
            boostValue = Mathf.Clamp(boostValue + 10, 0f, maxSpeed * 0.5f);
        }
        else 
        {
            //boostValue = Mathf.Max(0f, boostValue - 10f);
        }
    }
    public void setSpeedValues(float airplaneSpeed, float minSpeed, float maxSpeed) {
        this.airplaneSpeed = airplaneSpeed;
        this.minSpeed = minSpeed;
        this.maxSpeed = maxSpeed;
    }
    #endregion
    public float wheelbase = 4f;
    public Rigidbody vehicleNRigidbody;
    public float fixedDeltaTime = 0.02f;
    bool MouseHeel = false;
    float pointrot = 0f;
    float pointrot2 = 0;
    Vector2 _moveDirection;
    public float Speed2 = 10f;
    public float JumpForce = 300f;
    private bool _isGrounded;
    private Rigidbody _rb;
    [SerializeField] private float _lateralMovementSpeed;
    public Vector3 inertiaTensor = new Vector3(370f, 1640f, 1150f);
    [SerializeField] private float _longitudinalMovementSpeed;
    [SerializeField] LayerMask _layerMask;
    public static float timeSpent;
    private bool freezeWhileIdle;
    private float _sleepTimer;
    public float stopSpeed;
    public float turboSpeed;

    [Range(1, 30)]
    [Tooltip(
        "    Number of substeps when vehicle speed is\r\n    < 1m/ s.\r\n        Larger number reduces creep but decreases performance.")]
    public int lowSpeedSubsteps = 25;

    [Range(1, 30)] [Tooltip("    Number of physics substeps when vehicle speed is >= 1m/s.")]
    public int highSpeedSubsteps = 20;

    [Range(1, 20)] [Tooltip("    Number of physics substeps when vehicle is asleep.")]
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

    public void OnHeight1(InputAction.CallbackContext context)
    {
        GetComponent<Rigidbody>().AddForce(new Vector3(0,context.ReadValue<float>()*25,0) * 1);
    }

    public void OnHeight2(InputAction.CallbackContext context)
    {
        GetComponent<Rigidbody>().AddForce(new Vector3(0,context.ReadValue<float>()*25,0) * 1);
    }

   

    public void OnMove(InputAction.CallbackContext context)
    {
        //движение
        print("X");
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

        //if (vMove == 0){vMove = 0.3f;}

        _moveDirection = new Vector2(hMove, vMove * 2f);
    }

    // Use this for initialization
    void Start()
    {
        ApplyInitialRigidbodyValues();
        //vehicleNRigidbody.OnPrePhysicsSubstep += OnPrePhysicsSubstep;
        //vehicleNRigidbody.OnPhysicsSubstep += OnPhysicsSubstep;
        //vehicleNRigidbody.OnPostPhysicsSubstep += OnPostPhysicsSubstep;
        //if (powertrain.wheelGroups.Count == 2
        //    && powertrain.wheelGroups[0].Wheels.Count == 2
        //    && powertrain.wheelGroups[1].Wheels.Count == 2)
        {
            //wheelbase = Vector3.Distance(
                //powertrain.wheelGroups[0].LeftWheel.wheelController.transform.position,
                //powertrain.wheelGroups[1].LeftWheel.wheelController.transform.position);
        }
    }

    public virtual void OnPhysicsSubstep(float t, float dt, int i)
    {
        //if (_multiplayerInstanceType == MultiplayerInstanceType.Local)
        {
            //powertrain.OnPhysicsSubstep(t, dt, i);
        }
    }

    public virtual void OnPostPhysicsSubstep(float t, float dt)
    {
        //if (_multiplayerInstanceType == MultiplayerInstanceType.Local)
        {
            //powertrain.OnPostPhysicsSubstep(t, dt);
            //vehicleNRigidbody.Substeps =
            //    isAwake ? Speed < 2 ? lowSpeedSubsteps : highSpeedSubsteps : asleepSubsteps;
        }
        //else
        {
            //vehicleNRigidbody.Substeps = 1;
        }
    }

    public virtual void OnPrePhysicsSubstep(float t, float dt)
    {
        fixedDeltaTime = dt;
        ApplyLowSpeedFixes();
    }

    private void ApplyLowSpeedFixes()
    {
        // Increase inertia when still to mitigate jitter at low dt.
        

        //_sleepTimer += fixedDeltaTime;
    }

    // Update is called once per frame
    void Update()
    {
        //effectsManager.Update();
        //soundManager.Update();
        if (
            //GetComponent<Rigidbody>().isKinematic == true || 
            GetComponent<CollShip>()._pause)
        {
            return;
        }

        if (!MMDebug.Raycast3DBoolean(transform.position, new Vector3(0, -1, 0), 2.5f, _layerMask, Color.red, true))
        {
            //LongMovement(0.8f);
            return;
        }

        if (
            //GetComponent<CollShip>()._pause|| 
        GetComponent<CollShip>().IsMenu)
        {
            _moveDirection = Vector3.zero;
            print("z");
            return;
        }
        
        _rb = GetComponent<Rigidbody>();
        pointrot += _moveDirection.x*-100;
        //_rb.AddForce(new Vector3(0,_moveDirection.y*10,0) * 10);
        gameObject.transform.Rotate(0, _moveDirection.x * 10, 0);
        //* Time.deltaTime;
        //LateralRotate(pointrot);
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
        
    }
}