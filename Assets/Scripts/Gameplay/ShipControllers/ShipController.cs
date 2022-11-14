using Gameplay.ShipSystems;
using Gameplay.Spaceships;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;
#if UNITY_EDITOR
using UnityEditor;

#endif

namespace Gameplay.ShipControllers
{
    [System.Serializable]
    public abstract class ShipController : MonoBehaviour
    {
        
        private ISpaceship _spaceship;

        public void Init(ISpaceship spaceship)
        {
            _spaceship = spaceship;
        }

        private void Start()
        {
            ProcessMove();
            destTime = 0;
        }
        float destTime = 0;
        private void Update()
        {
            ProcessHandling(_spaceship.MovementSystem);
            ProcessFire(_spaceship.WeaponSystem);
            if (tag == "bonus") 
            {
                destTime += Time.deltaTime;
                //print(destTime);
                if (destTime >= 6.5f) { print("destroy"); Destroy(this.gameObject); destTime = 0; }
            }
        }
        private void OnCollisionEnter(Collision collision) 
        {
            //Collision1(collision);
        }

        protected abstract void Collision1(Collision collision);
        protected abstract void ProcessMove();
        protected abstract void ProcessHandling(MovementSystem movementSystem);
        protected abstract void ProcessFire(WeaponSystem fireSystem);
    }
}
