using Gameplay.ShipSystems;
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
        private void Awake()
        {
            //powertrain.Awake(this);
        }
        public void OnMove(InputAction.CallbackContext context)
        {

            float hMove = context.ReadValue<Vector2>().x;
            float vMove = context.ReadValue<Vector2>().y;
            if (vMove < 0 || vMove > 0)
            {
                //    isVMove = true; }
                //if (isVMove) { 
                timeSpent += Time.deltaTime;
                if (timeSpent < 3f)
                {
                    float remainder = timeSpent % .8f;
                    vMove = 0.8f;
                }
                else
                {
                    isVMove = false;
                }

            }
            if (vMove == 0/*&& !isVMove*/) { vMove = 0.1f; }
            _moveDirection = new Vector2(hMove, vMove * 2f);
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
        void OnCollisionEnter(Collision collision)
        {
            IsGroundedUpate(collision, true);
        }

        void OnCollisionExit(Collision collision)
        {
            IsGroundedUpate(collision, false);
        }

        private void IsGroundedUpate(Collision collision, bool value)
        {
            if (collision.gameObject.tag == ("Ground"))
            {
                _isGrounded = value;
            }
        }
        public void HorizontalVertical(InputAction.CallbackContext context)
        {
            _moveDirection = context.ReadValue<Vector2>();
        }
        public void RotZero()
        {
            pointrot = 0f;
            pointrot2 = 0f;
        }
        protected override void ProcessHandling(MovementSystem movementSystem)
        {
            if (GetComponent<Rigidbody>().isKinematic == true || GetComponent<CollShip>()._pause) { return; }
            if (!MMDebug.Raycast3DBoolean(transform.position, new Vector3(0, -1, 0), 2.5f, _layerMask, Color.red, true))
            {
                movementSystem.LongMovement(0.8f);
                return;
            }
            if (GetComponent<CollShip>()._pause || GetComponent<CollShip>().IsMenu)
            {
                _moveDirection = Vector3.zero;
                return;
            }
            _rb = GetComponent<Rigidbody>();
            //MovementLogic();
            //OnMove();
            pointrot += _moveDirection.x * Time.deltaTime * -100;
            movementSystem.LateralRotate(pointrot);
            movementSystem.LongMovement(_moveDirection.y);

            //if(
            //   transform.position.x<GetComponent<CollShip>().limitx||
            //   transform.position.x>GetComponent<CollShip>().limitx1
            //   )
            //{
            //    movementSystem.LateralMovement(Input.GetAxis("Horizontal") * Time.deltaTime);
            //    if (MouseHeel) { /*print((Input.mousePosition.x * 0.1f) - 50);*/ transform.position = new Vector3((Input.mousePosition.x*0.1f)-40, transform.position.y, transform.position.z); }
            //}
            //if(
            //   transform.position.x > GetComponent<CollShip>().limitx)
            //{
            //    //print("x");
            //    transform.position = new Vector3(GetComponent<CollShip>().limitx-2, transform.position.y, transform.position.z);
            //}

            //if(transform.position.x < GetComponent<CollShip>().limitx1
            //   )
            //{
            //    //print("x1");
            //    transform.position = new Vector3(GetComponent<CollShip>().limitx1 + 2, transform.position.y, transform.position.z);
            //}

        }

        protected override void ProcessFire(WeaponSystem fireSystem)
        {
            //if (Input.GetKey(KeyCode.Space)|| Input.GetMouseButton(0))
            //{
            //    fireSystem.TriggerFire();
            //    var source = GetComponent<AudioSource>();
            //    if(source !=null)source.PlayOneShot(source.clip);
            //}
            //if (Input.GetKey(KeyCode.M))
            //{
            //    MouseHeel = true;
            //}
            //if (Input.GetKey(KeyCode.N))
            //{
            //    MouseHeel = false;
            //}
        }

    }
}