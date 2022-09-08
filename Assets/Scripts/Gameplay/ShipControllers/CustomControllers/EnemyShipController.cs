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

    private float speed, dangerZone;
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

    public bool canFollow() {
        return GameObject.FindGameObjectWithTag("Airplane").transform != null &&
               Vector3.Distance(transform.position, GameObject.FindGameObjectWithTag("Airplane").transform.position) <= dangerZone;
    }
    private IEnumerator startMove() {
        while (true) {
            checkTerrainCollision();
            if (GameObject.FindGameObjectWithTag("Airplane") != null)
            {
                if (canFollow()) // can follow the fighter
                    chaseTheFighter(speed);
                else if (moveSpots != null && moveSpots.Length > 0)
                {
                    patrolMove();
                }
            }

            yield return null;
        }
    }
    private void chaseTheFighter(float speed) {
        //float turn = 20f;
        //rb.velocity = transform.forward * speed;
        //var rocketTargetRotation = Quaternion.LookRotation(GameObject.FindGameObjectWithTag("Airplane").transform.position - transform.position);
        //Quaternion.RotateTowards(transform.rotation, rocketTargetRotation, turn);
        //rb.MovePosition(GameObject.FindGameObjectWithTag("Airplane").transform.position);
        transform.LookAt(GameObject.FindGameObjectWithTag("Airplane").transform.position);
        Vector3 Moveto = transform.forward * speed * Time.deltaTime;
        controller.Move(Moveto);//print("move2"+dangerZone);
    }

    private void patrolMove() {
        
        if (targetSpot == -1)
            targetSpot = Random.Range(0, moveSpots.Length);
        else if (Vector3.Distance(transform.position, moveSpots[targetSpot].transform.position) < 0.2f) { // enemy reached to the spot
            print("patrol"+targetSpot);
            targetSpot = (targetSpot + 1) % moveSpots.Length;
        }
        
        if (!canFollow()) 
            goToSpot(targetSpot, speed);
    }
    private void goToSpot(int targetSpot, float speed) {
        if (targetSpot < 0 || targetSpot > moveSpots.Length)
            return;
        transform.LookAt(moveSpots[targetSpot].transform.position);
        Vector3 moveTo = transform.forward * speed * Time.deltaTime;
        controller.Move(moveTo);
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
    }
    protected override void ProcessHandling(MovementSystem movementSystem)
    {
        checkTerrainCollision();
        if (GameObject.FindGameObjectWithTag("Airplane") != null)
        {
            if (canFollow()) // can follow the fighter
                chaseTheFighter(speed);
            else if (moveSpots != null && moveSpots.Length > 0)
            {
                patrolMove();
            }
        }
        if (moveSpots == null || moveSpots.Length == 0)
            moveSpots = GameObject.FindGameObjectsWithTag("EnemyMoveSpot");
        speed = 85f;
        dangerZone = 1080f;
    }
    protected override void ProcessFire(WeaponSystem fireSystem)
    {
        if (!_fire)return;
        //fireSystem.TriggerFire();
        if (canAttack())
        {
            _attack.attack();
        }
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
