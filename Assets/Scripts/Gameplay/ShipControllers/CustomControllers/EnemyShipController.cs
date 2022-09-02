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


    private float speed, dangerZone;
    private Enemy enemy = new Enemy(150f,195f,25f,1080f);
    [SerializeField] private Attack _attack;
    private int targetSpot = -1;
    private bool _fire = true;
    protected int anotherMovement = 0;
    public void setAnotherMovement(int any)
    {
        anotherMovement = any;
    } 
    protected override void ProcessMove() {
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
        speed = enemy != null ? enemy.speed : 0f;
        dangerZone = enemy != null ? enemy.dangerZone : 0f;
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
