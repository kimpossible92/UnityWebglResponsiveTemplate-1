using System.Collections;
using System.Collections.Generic;
using Gameplay.ShipControllers;
using Gameplay.ShipSystems;

using UnityEngine;

public class EnemyShipController : ShipController
{
    [SerializeField]
    private Vector2 _fireDelay;

    //private EnemyManager manager;

    public CharacterController controller;
    public Rigidbody rb;
    public GameObject[] moveSpots;
    private HealthHandler healthHandler;

    [SerializeField]private float speed, dangerZone;
    private Enemy enemy = new Enemy(150f,85f,25f,1080f);
    [SerializeField] private Attack _attack;
    private int targetSpot = -1;
    private bool _fire = true;
    protected int anotherMovement = 0;
    public void setAnotherMovement(int any)
    {
        anotherMovement = any;
    } 
    protected override void ProcessMove()
    {
        healthHandler = GetComponent<HealthHandler>();
        //ZplusPosition = transform.position.z;
        //StartCoroutine(startMove());
    }
    private void checkTerrainCollision() {
        if (Terrain.activeTerrain != null) {
            float height = Terrain.activeTerrain.SampleHeight(transform.position);
            if (height > transform.position.y) {
                transform.position = new Vector3(transform.position.x, height, transform.position.z);
            }
        }

    }

    protected override void Collision1(Collision collision)
    {
        Collider collider = collision.collider;
        BaseFire fire = collider.GetComponent<BaseFire>();
        if (fire != null && !fire.createdBy.CompareTag(gameObject.tag)) {
            healthHandler.takdeDamage(fire.damage);
        }
        if (collider.CompareTag("Terrain")) {
            healthHandler.takdeDamage(10f);
        }
        if (collider.CompareTag("Airplane"))
        {
            healthHandler.takdeDamage(200f);
        }
        if (collider.CompareTag("Fire"))
        {
            healthHandler.takdeDamage(30f);
        }
    }
    public float YrandomSpot = 0;
    public float ZplusPosition = 0;
    protected float fullAddZposition = 0;
    [HideInInspector] int randInt;
    protected override void ProcessHandling(MovementSystem movementSystem)
    {
        if (GetComponent<EnemySp>().GetSpawner!=null)
        {
            if (transform.position.z < GetComponent<EnemySp>().GetSpawner.transform.position.z - 450) { transform.position = GetComponent<EnemySp>().GetSpawner.transform.position; }
        }
        if (GetComponent<EnemySp>().GetSpawner2 != null)
        {
            if (transform.position.z < GetComponent<EnemySp>().GetSpawner2.transform.position.z - 450) { transform.position = GetComponent<EnemySp>().GetSpawner2.transform.position; }
        }
        if (anotherMovement == 1) {
            controller.Move(Vector3.back * (speed + (speed > 0f ? 10 : 0)) * Time.deltaTime);
            //movementSystem.LateralMovement(Time.deltaTime); 
        }
        else if (anotherMovement == -1) { movementSystem.HorizontalMovement(Time.deltaTime); }
        else if (anotherMovement == -2) { }
        else { movementSystem.LongitudinalMovement(Time.deltaTime); }
    }
    protected override void ProcessFire(WeaponSystem fireSystem)
    {
        if (!_fire)return;
        fireSystem.TriggerFire();
        //controller.Move(new Vector3(0, YrandomSpot, ZplusPosition) * (speed + (speed > 0f ? 10 : 0)) * Time.deltaTime);
        StartCoroutine(FireDelay(Random.Range(_fireDelay.x, _fireDelay.y)));
        
    }
    private bool canAttack() {
        return Vector3.Distance(transform.position, GameObject.FindGameObjectWithTag("Airplane").transform.position) <= dangerZone;
    }
    
    private IEnumerator FireDelay(float delay)
    {
        _fire = false;
        yield return new WaitForSeconds(delay);
        _fire = true;
    }
}
