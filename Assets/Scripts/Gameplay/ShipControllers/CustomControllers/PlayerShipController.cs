using Gameplay.ShipSystems;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;
using UnityEngine.InputSystem;
#if UNITY_EDITOR
using UnityEditor;

#endif
namespace Gameplay.ShipControllers.CustomController
{
    public class PlayerShipController : ShipController
    {
        bool MouseHeel = false;
        float pointrot = 0f; float pointrot2 = 0;
        Vector2 _moveDirection;
        public float Speed2 = 10f;
        public float JumpForce = 300f;
        private bool _isGrounded;
        private Rigidbody _rb;
        [SerializeField] LayerMask _layerMask; public static float timeSpent;
        protected bool isVMove = false;
        Vector3 startedPos;
        private Transform cachedTransform;
        private void Awake()
        {
            startedPos = transform.position; 
            cachedTransform = transform;
        }
        protected override void Collision1(Collision collision)
        {

        }
        protected override void ProcessMove() {
            //StartCoroutine(startMove());
        }
        public void OnMove()
        {
            if (GetComponent<CollShip>()._pause)
            {
                _moveDirection = Vector3.zero;
                return;
            }
            float hMove = Input.GetAxis("Horizontal");
            float vMove = Input.GetAxis("Vertical");
            _moveDirection = new Vector2(hMove, vMove);
        }
        private void MovementLogic()
        {
            if (GetComponent<Rigidbody>().isKinematic == true || GetComponent<CollShip>()._pause) { return; }
            if (!MMDebug.Raycast3DBoolean(transform.position, new Vector3(0, -1, 1), 2.5f, _layerMask, Color.red, true))
            {
                return;
            }
            float moveHorizontal = Input.GetAxis("Horizontal");

            float moveVertical = Input.GetAxis("Vertical");

            Vector3 movement = new Vector3(0.0f, 0.0f, moveVertical);
            //transform.Translate(movement * Speed * Time.fixedDeltaTime);
            //_rb.velocity = (movement * Speed);
            _rb.AddForce(-movement * Speed2);
        }
        private void JumpLogic()
        {
            if (Input.GetAxis("Jump") > 0)
            {
                if (_isGrounded)
                {
                   _rb.AddForce(Vector3.up * JumpForce);
                }
            }
        }
        void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision != null && collision.CompareTag("wall"))
            {
                startedPos = transform.position;
            }
            //IsGroundedUpate(collision, true);
        }

        void OnCollisionExit(Collision collision)
        {
            //IsGroundedUpate(collision, false);
        }

        private void IsGroundedUpate(Collision collision, bool value)
        {
            if (collision.gameObject.tag == ("Ground"))
            {
                _isGrounded = value;
            }
        }
        public void RotZero()
        {
            pointrot = 0f;
            pointrot2 = 0f;
        }
        private float yaw = 0f, pitch = 0f, roll = 0f;
        float horizontal = 0;
        public float minRollAngle = -45f, maxRollAngle = 45f;
        float vertical = 0;
        public float rotateSpeed = 20f;
        public float airplaneSpeed;
        float maxSpeed = 180f, minSpeed = 60f, speedAcceleration = 3f;
        private float boostValue; 
        public Vector2 jumpForce = new Vector2(0, 250); 
        public AudioClip jumpSound; 
        private Vector2 birdPosition;
        #region ApplayForward
        private void handleBoost()
        {
            if (Input.GetKey(KeyCode.LeftShift))
            {
                boostValue = Mathf.Clamp(boostValue + 10, 0f, maxSpeed * 0.5f);
            }
            else
            {
                boostValue = Mathf.Max(0f, boostValue - 10f);
            }
        }
        private void applayForwardMovement()
        {
            Vector3 direction = -transform.right.normalized;
            airplaneSpeed -= transform.forward.y * speedAcceleration * Time.deltaTime;
            airplaneSpeed = Mathf.Clamp(airplaneSpeed, minSpeed, maxSpeed);
            transform.Translate(direction* (airplaneSpeed + (airplaneSpeed > 0f ? boostValue : 0f)) * Time.deltaTime);
            //controller.Move(direction * (airplaneSpeed + (airplaneSpeed > 0f ? boostValue : 0f)) * Time.deltaTime);
        }
        #endregion
        protected override void ProcessHandling(MovementSystem movementSystem)
        {
            OnMove();
            ProcessHandle();
            Camera.main.transform.position = new Vector3(transform.position.x,
                Camera.main.transform.position.y, Camera.main.transform.position.z);

            {
                //    if (
                //       transform.position.x < GetComponent<CollShip>().limitx ||
                //       transform.position.x > GetComponent<CollShip>().limitx1
                //       )
                //    {
                //        movementSystem.LateralMovement(Input.GetAxis("Horizontal") * Time.deltaTime);
                //        if (MouseHeel) { /*print((Input.mousePosition.x * 0.1f) - 50);*/ transform.position = new Vector3((Input.mousePosition.x * 0.1f) - 40, transform.position.y, transform.position.z); }
                //    }
                //    if (
                //       transform.position.x > GetComponent<CollShip>().limitx)
                //    {
                //        //print("x");
                //        transform.position = new Vector3(GetComponent<CollShip>().limitx - 2, transform.position.y, transform.position.z);
                //    }

                //    if (transform.position.x < GetComponent<CollShip>().limitx1
                //       )
                //    {
                //        //print("x1");
                //        transform.position = new Vector3(GetComponent<CollShip>().limitx1 + 2, transform.position.y, transform.position.z);
                //    }
            }
        }
        public void ProcessHandle()
        {
            if (Input.GetKeyUp(KeyCode.Space) || Input.GetKeyUp(KeyCode.Mouse0))//|| (Input.touchCount == 1 && Input.touches[0].phase == TouchPhase.Began))
            {
                GetComponent<Rigidbody2D>().velocity = Vector2.zero;
                GetComponent<Rigidbody2D>().AddForce(jumpForce);
                AudioSource.PlayClipAtPoint(jumpSound, new Vector3(0, 0, 0), 0.2f);
            }
            birdPosition = Camera.main.WorldToScreenPoint(cachedTransform.position);
            //applayForwardMovement();
            //handleRotatoin();
            //handleBoost();
        }
        public void handleRotatoin()
        {
            if (Input.GetMouseButton(0))
            {
                vertical = 1f;
            }
            else
            {
                vertical = -1f;
            }
            pointrot += _moveDirection.x * Time.deltaTime * -100;
            roll += horizontal;
            pitch += (-vertical) * rotateSpeed * Time.deltaTime;
            yaw += horizontal * rotateSpeed * Time.deltaTime;
            roll = Mathf.Clamp(roll, minRollAngle, maxRollAngle);
            transform.rotation = Quaternion.Euler(new Vector3(-roll, yaw, pitch));
        }
        protected override void ProcessFire(WeaponSystem fireSystem)
        {
            if (Input.GetKey(KeyCode.Space) || Input.GetMouseButton(0))
            {
                fireSystem.TriggerFire();
                var source = GetComponent<AudioSource>();
                if (source != null) source.PlayOneShot(source.clip);
            }
            if (Input.GetKey(KeyCode.M))
            {
                MouseHeel = true;
            }
            if (Input.GetKey(KeyCode.N))
            {
                MouseHeel = false;
            }
        }

    }
}