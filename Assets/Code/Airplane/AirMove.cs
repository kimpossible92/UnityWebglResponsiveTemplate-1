//using Assets.Code.utils;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AirMove : MonoBehaviour {

    public CharacterController controller;

    public float airplaneSpeed;
    public float maxSpeed = 180f;
    public float minSpeed = 60f;
    public float speedAcceleration = 3f;
    public float rotateSpeed = 20f;
    [SerializeField] private bool is_interpolate;
    private float boostValue;
    
    public float minRollAngle = -45f, maxRollAngle = 45f;
    private float yaw = 0f, pitch = 0f, roll = 0f;
    public float resetRotationSpeed = 100f;
    #region Attack1
    public Attack basicAttack;
    public Attack specialAttack;
    private bool _shield = true;
    public bool Shiled() { return _shield; }
    [SerializeField] private GameObject Cube;
    [SerializeField]
    private Vector2 _shieldDelay;
    void Start()
    {
        Cube.gameObject.SetActive(false);
    }
    private IEnumerator FireDelay(float delay)
    {
        _shield = false;
        yield return new WaitForSeconds(delay);
        Cube.gameObject.SetActive(false);
        _shield = true;
    }
    // Update is called once per frame
    void Update()
    {
        if (basicAttack != null)
        {
            if (Input.GetKey(KeyCode.Mouse0)){basicAttack.attack();}
        }

        if (specialAttack != null)
        {
            //if (Input.GetKeyDown(KeyCode.Mouse0)){ specialAttack.attack();}
        }
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (!_shield) return;
            Cube.gameObject.SetActive(true);
            StartCoroutine(FireDelay(Random.Range(_shieldDelay.x, _shieldDelay.y)));
        }
    }

    public void setDamage(float basicDamage, float specialDamage)
    {
        if (basicAttack != null)
            basicAttack.damage = basicDamage;
        if (specialAttack != null)
            specialAttack.damage = specialDamage;
    }
    #endregion

    private void applayForwardMovement() {
        Vector3 direction = transform.forward.normalized;
        airplaneSpeed -= transform.forward.y * speedAcceleration * Time.deltaTime;
        airplaneSpeed = Mathf.Clamp(airplaneSpeed, minSpeed, maxSpeed);
        controller.Move(direction * (airplaneSpeed + (airplaneSpeed > 0f  ? boostValue : 0f)) * Time.deltaTime);
    }


    private void handleRotatoin() {
        // Rotation
        int interpolate = 1;
        if (is_interpolate) interpolate = -1;
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = interpolate*Input.GetAxis("Vertical");

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

    private void handleBoost() {
        if (Input.GetKey(KeyCode.LeftShift)) {
            boostValue = Mathf.Clamp(boostValue + 10, 0f, maxSpeed * 0.5f);
        } else {
            boostValue = Mathf.Max(0f, boostValue - 10f);
        }
    }

    void FixedUpdate() {
        applayForwardMovement();

        handleRotatoin();

        handleBoost();
    }

    public void setSpeedValues(float airplaneSpeed, float minSpeed, float maxSpeed) {
        this.airplaneSpeed = airplaneSpeed;
        this.minSpeed = minSpeed;
        this.maxSpeed = maxSpeed;
    }

}