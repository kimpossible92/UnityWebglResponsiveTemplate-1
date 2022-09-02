using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class NewRigidBody : MonoBehaviour
{
    public Rigidbody targetRigidbody;
    public Transform targetTransform;

    public float dt, t;
    [Tooltip("    Mass in [kg].")]
    public float mass;
    [Tooltip("    Angular drag. Equivalent to rigidbody.angularDrag.")]
    public float angularDrag;
    [Tooltip("    Angular velocity in rad/s.")]
    public Vector3 angularVelocity;
    [Tooltip("    Center of mass of the body in local coordinates.")]
    public Vector3 centerOfMass;
    [Tooltip("    Linear drag. Equivalent to rigidbody.drag.")]
    public float drag;
    [Tooltip("    Inertia of the body. Equivalent to rigidbody.inertiaTensor.")]
    public Vector3 inertia;
    [Tooltip("    Position of the NRigidbody in world coordinates.")]
    public Vector3 nPosition;
    //     Rotation of the NRigidbody in world coordinates.
    public Quaternion nRotation;
    //     NRigidbody velocity in m/s in world coordinates.
    public Vector3 velocity;
    //     Equivalent to Rigidbody.useGravity.
    public bool useGravity;
    //     TRS matrix for local to world conversion.
    public Matrix4x4 localToWorldMatrix;
    [SerializeField] private int substeps = 4;
    private int _tmpSubsteps = -1;

    private Vector3 _angularImpulse;
    private Vector3 _linearImpulse;
    private Vector3 _totalAngularImpulse;
    private Vector3 _totalLinearImpulse;
    private float _invMass;
    private Vector3 _invInertia;
    private Vector3 _zeroVector;

    private Vector3 _initNPosition;
    private Quaternion _initNRotation;
    private Vector3 _stepInitNPosition;
    private Quaternion _stepInitNRotation;

    private Vector3 _stepPositionDelta;
    private Vector3 _worldInvInertia;
    public int Substeps
    {
        get { return substeps; }
        set { _tmpSubsteps = value < 1 ? 1 : value > 50 ? 50 : value; }
    }
    public Vector3 StepPositionDelta
    {
        get { return _stepPositionDelta; }
    }
    public Quaternion StepRotationDelta { get; private set; }
    public Vector3 TotalPositionDelta { get; private set; }
    public Quaternion TotalRotationDelta { get; private set; }
    public Vector3 TransformPosition
    {
        get { return nPosition - localToWorldMatrix.MultiplyVector(targetRigidbody.centerOfMass); }
    }
    public Quaternion TransformRotation
    {
        get { return nRotation; }
    }
    public Vector3 TransformUp { get; private set; }
    public Vector3 TransformRight { get; private set; }
    public Vector3 TransformForward { get; private set; }
    private void Awake()
    {
        targetTransform = transform;
        targetRigidbody = GetComponent<Rigidbody>();
    }


    private void Start()
    {
     //   SyncAllFromTarget();

        _tmpSubsteps = substeps;
        _totalAngularImpulse = Vector3.zero;
        _totalLinearImpulse = Vector3.zero;
        _zeroVector = Vector3.zero;
    }
    private void FixedUpdate()
    {
        localToWorldMatrix = transform.localToWorldMatrix;
        SyncMovementFromTarget();

        substeps = _tmpSubsteps;
        bool initSyncTransforms = Physics.autoSyncTransforms;
        Physics.autoSyncTransforms = false;

        float fixedTime = Time.fixedTime;
        float fixedDeltaTime = Time.fixedDeltaTime;
        t = fixedTime;

        if (substeps <= 0)
        {
            substeps = 1;
        }

        dt = fixedDeltaTime / substeps;

        if (dt < 1e-5f)
        {
            return;
        }

        _initNPosition = nPosition;
        _initNRotation = nRotation;

        _worldInvInertia = localToWorldMatrix.MultiplyVector(_invInertia);
        _worldInvInertia.x = _worldInvInertia.x < 0 ? -_worldInvInertia.x : _worldInvInertia.x;
        _worldInvInertia.y = _worldInvInertia.y < 0 ? -_worldInvInertia.y : _worldInvInertia.y;
        _worldInvInertia.z = _worldInvInertia.z < 0 ? -_worldInvInertia.z : _worldInvInertia.z;

        TransformForward = targetTransform.forward;
        TransformRight = targetTransform.right;
        TransformUp = targetTransform.up;

        OnPrePhysicsSubstep?.Invoke(fixedTime, fixedDeltaTime);

        // Run 
        for (int i = 0; i < substeps; i++)
        {
            _stepInitNPosition = nPosition;
            _stepInitNRotation = nRotation;

            OnPhysicsSubstep?.Invoke(t, dt, i);
            Step();

            _stepPositionDelta.x = nPosition.x - _stepInitNPosition.x;
            _stepPositionDelta.y = nPosition.y - _stepInitNPosition.y;
            _stepPositionDelta.z = nPosition.z - _stepInitNPosition.z;

            StepRotationDelta = (nRotation * Quaternion.Inverse(_stepInitNRotation)).normalized;
            localToWorldMatrix *= Matrix4x4.TRS(StepPositionDelta, StepRotationDelta, Vector3.one);

            TransformForward = StepRotationDelta * TransformForward;
            TransformRight = StepRotationDelta * TransformRight;
            TransformUp = StepRotationDelta * TransformUp;

            t += dt;
        }

        TotalPositionDelta = nPosition - _initNPosition;
        TotalRotationDelta = (nRotation * Quaternion.Inverse(_initNRotation)).normalized;

        OnPostPhysicsSubstep?.Invoke(t, fixedDeltaTime);

        targetRigidbody.AddForce(_totalLinearImpulse, ForceMode.Impulse);
        targetRigidbody.AddTorque(_totalAngularImpulse, ForceMode.Impulse);

        _totalLinearImpulse = _zeroVector;
        _totalAngularImpulse = _zeroVector;

        Physics.autoSyncTransforms = initSyncTransforms;
    }
    public event Action<float, float, int> OnPhysicsSubstep;
    public event Action<float, float> OnPrePhysicsSubstep;
    public event Action<float, float> OnPostPhysicsSubstep;
    private void Step()
    {
        // Apply gravity
        if (useGravity)
        {
            velocity.x += Physics.gravity.x * dt;
            velocity.y += Physics.gravity.y * dt;
            velocity.z += Physics.gravity.z * dt;
        }

        // Apply impulses
        float invMassDt = _invMass * dt;
        velocity.x += _linearImpulse.x * invMassDt;
        velocity.y += _linearImpulse.y * invMassDt;
        velocity.z += _linearImpulse.z * invMassDt;

        angularVelocity.x += _angularImpulse.x * _worldInvInertia.x * dt;
        angularVelocity.y += _angularImpulse.y * _worldInvInertia.y * dt;
        angularVelocity.z += _angularImpulse.z * _worldInvInertia.z * dt;

        // Apply drag
        float dragMultiplier = 1.0f - drag * dt;
        if (dragMultiplier < 0.0f)
        {
            dragMultiplier = 0.0f;
        }

        velocity.x *= dragMultiplier;
        velocity.y *= dragMultiplier;
        velocity.z *= dragMultiplier;

        float dragDt = angularDrag * dt;
        angularVelocity.x -= angularVelocity.x * dragDt;
        angularVelocity.y -= angularVelocity.y * dragDt;
        angularVelocity.z -= angularVelocity.z * dragDt;

        // Apply velocity
        Vector3 eulerRotation = angularVelocity * (Mathf.Rad2Deg * dt);
        nRotation = Quaternion.Euler(eulerRotation) * nRotation;

        nPosition.x += velocity.x * dt;
        nPosition.y += velocity.y * dt;
        nPosition.z += velocity.z * dt;

        _totalLinearImpulse.x += _linearImpulse.x * dt;
        _totalLinearImpulse.y += _linearImpulse.y * dt;
        _totalLinearImpulse.z += _linearImpulse.z * dt;

        _totalAngularImpulse.x += _angularImpulse.x * dt;
        _totalAngularImpulse.y += _angularImpulse.y * dt;
        _totalAngularImpulse.z += _angularImpulse.z * dt;

        // Reset impulses
        _angularImpulse = _zeroVector;
        _linearImpulse = _zeroVector;
    }
    
    public void SyncAllFromTarget()
    {
        localToWorldMatrix = targetTransform.localToWorldMatrix;

        useGravity = targetRigidbody.useGravity;

        angularDrag = targetRigidbody.angularDrag;
        angularVelocity = targetRigidbody.angularVelocity;
        centerOfMass = targetRigidbody.centerOfMass;
        mass = targetRigidbody.mass;
        drag = targetRigidbody.drag;
        inertia = targetRigidbody.inertiaTensor;
        nRotation = targetRigidbody.rotation;
        nPosition = targetRigidbody.position + localToWorldMatrix.MultiplyVector(targetRigidbody.centerOfMass);
        velocity = targetRigidbody.velocity;

        _invMass = 1f / mass;
        _invInertia.x = inertia.x == 0 ? 1e-8f : 1f / inertia.x;
        _invInertia.y = inertia.y == 0 ? 1e-8f : 1f / inertia.y;
        _invInertia.z = inertia.z == 0 ? 1e-8f : 1f / inertia.z;
    }


    private void SyncMovementFromTarget()
    {
        nRotation = targetRigidbody.rotation;
        nPosition = targetRigidbody.position + localToWorldMatrix.MultiplyVector(targetRigidbody.centerOfMass);
        velocity = targetRigidbody.velocity;
        angularVelocity = targetRigidbody.angularVelocity;
        centerOfMass = targetRigidbody.centerOfMass;
        drag = targetRigidbody.drag;
        angularDrag = targetRigidbody.angularDrag;

        mass = targetRigidbody.mass;
        _invMass = 1f / mass;

        inertia = targetRigidbody.inertiaTensor;
        _invInertia.x = inertia.x == 0 ? 1e-8f : 1f / inertia.x;
        _invInertia.y = inertia.y == 0 ? 1e-8f : 1f / inertia.y;
        _invInertia.z = inertia.z == 0 ? 1e-8f : 1f / inertia.z;
    }
}
