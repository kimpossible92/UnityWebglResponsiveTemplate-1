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
        }
        private void Update()
        {
            ProcessHandling(_spaceship.MovementSystem);
            ProcessFire(_spaceship.WeaponSystem);
            
        }

        protected abstract void ProcessMove();
        protected abstract void ProcessHandling(MovementSystem movementSystem);
        protected abstract void ProcessFire(WeaponSystem fireSystem);
    }
}
