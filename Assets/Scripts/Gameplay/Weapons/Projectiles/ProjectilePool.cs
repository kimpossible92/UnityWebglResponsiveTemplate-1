using System;
using Gameplay.Helpers;
using UnityEngine;

namespace Gameplay.Weapons.Projectiles
{
    public abstract class ProjectilePool : MonoBehaviour, IDamageDealer
    {
        #region Interface
        public void ReturnToPool()
        {
            gameObject.SetActive(false);
        }
        #endregion
        [SerializeField]
        private float _speed;

        [SerializeField] 
        private float _damage;

        [SerializeField]
        private UnitBattleIdentity _battleIdentity;


        public UnitBattleIdentity BattleIdentity => _battleIdentity;
        public float Damage => _damage;

        

        public void Init(UnitBattleIdentity battleIdentity)
        {
            _battleIdentity = battleIdentity;
        }
        

        private void Update()
        {
            if(GetComponent<BulletFire>()==null)Move(_speed);
        }

        
        private void OnCollisionEnter(Collision other)
        {
            if(other.gameObject.tag=="Airplane"){}
            var damagableObject = other.gameObject.GetComponent<IDamagable>();
            
            if (damagableObject != null 
                //&& damagableObject.BattleIdentity != BattleIdentity 
                && other.gameObject.tag !="bonus")
            {            
                //print("Dest");
                damagableObject.ApplyDamage(this);
            }
        }
        


        protected abstract void Move(float speed);
    }
}
