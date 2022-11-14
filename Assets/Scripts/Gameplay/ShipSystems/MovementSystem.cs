using UnityEngine;

namespace Gameplay.ShipSystems
{
    public class MovementSystem : MonoBehaviour
    {

        [SerializeField]
        private float _lateralMovementSpeed;
        
        [SerializeField]
        private float _longitudinalMovementSpeed;
    
        //public void loadMove()
        //{
        //    _lateralMovementSpeed = 250.0f;
        //    Invoke("oldmove", 8.0f);
        //}
        public void LongMovement(float amount)
        {
            Move(amount * 10, Vector2.left);
        }
        public void LateralRotate(float amount)
        {
            transform.rotation = Quaternion.Euler(0, 0, -amount);
        }
        public void LateralMovement(float amount)
        {
            if (tag == "bonus") { if (transform.position.y < -5.2f) { } else { Move(amount * _longitudinalMovementSpeed, Vector3.back); } }
            else Move(amount * _lateralMovementSpeed, Vector3.back);
        }

        public void LongitudinalMovement(float amount)
        {
            if (tag == "bonus") { if (transform.position.y < -5.2f) { } else { Move(amount * _longitudinalMovementSpeed, Vector3.up); } }
            else { Move(amount * _longitudinalMovementSpeed, Vector3.up); }
        }
        public void HorizontalMovement(float amount)
        {
            if (tag == "bonus") { if (transform.position.y < -5.2f) { } else { Move(amount * _longitudinalMovementSpeed, Vector3.up); } }
            else Move(amount * _longitudinalMovementSpeed, Vector3.left);
        }
        private void oldmove()
        {
            _lateralMovementSpeed = 50.0f;
        }
        private void Move(float amount, Vector3 axis)
        {
            transform.Translate(amount * axis * Time.deltaTime);
            //transform.Rotate(-amount * axis * Time.deltaTime, 1f);
            //transform.rotation = Quaternion.Euler(-amount * Time.deltaTime * -100, 0, 0);
        }
        private Vector3 vectorPosition;
        public Vector3 _vecPosition => vectorPosition;
        private void SetOnVector()
        {
            vectorPosition = transform.position;
        }
    }
}
