using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Gameplay.Weapons;

public class BulletFire : BaseFire
{
    public GameObject imbactEffect;
    [SerializeField]
    private float _speed;

    [SerializeField]
    private float _damage;
    private UnitBattleIdentity _battleIdentity;
    public Gameplay.Weapons.UnitBattleIdentity BattleIdentity => _battleIdentity;
    public float Damage => _damage;
    private void Start() {
    }
    private void Update()
    {
        if (transform.position.z > FindObjectOfType<AirManager>().transform.position.z + 170)
        {
            Destroy(gameObject);
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        GameObject imbact = Instantiate(imbactEffect, transform.position, Quaternion.LookRotation(collision.contacts[0].normal));
        Destroy(imbact, 2f);
        Destroy(gameObject);
        if (collision.gameObject.tag == "Airplane") { }
        var damagableObject = collision.gameObject.GetComponent<Gameplay.Weapons.IDamagable>();

        if (damagableObject != null
            && damagableObject.BattleIdentity != BattleIdentity && collision.gameObject.tag != "bonus")
        {

            //damagableObject.ApplyDamage(this);
        }
    }
}
