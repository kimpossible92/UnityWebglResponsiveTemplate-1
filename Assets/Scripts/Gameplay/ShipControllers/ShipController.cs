using Gameplay.ShipSystems;
using Gameplay.Spaceships;
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

        private void Start(){
            // Calculate wheelbase if there are 2x2 wheels
            //powertrain.Awake(this);
        }
        private void Update()
        {
            ProcessHandling(_spaceship.MovementSystem);
            ProcessFire(_spaceship.WeaponSystem);
            
        }

        protected abstract void ProcessHandling(MovementSystem movementSystem);
        protected abstract void ProcessFire(WeaponSystem fireSystem);
    }
}
