using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Session {
    public int id;
    public int airplaneId;
    public int environmentId;
    public string sessionName;
    public GameState gameState;

    public Session(int airplaneId, int environmentId, string sessionName, GameState gameState) {
        this.id = DateTime.Now.Millisecond;
        this.airplaneId = airplaneId;
        this.environmentId = environmentId;
        this.sessionName = sessionName;
        this.gameState = gameState;
    }

}

[Serializable]
public class GameState {
    public float health;
    public int difficultyLevel;
    public int coins;
    public int score;

    public GameState(float health, int difficultyLevel, int coins, int score) {
        this.health = health;
        this.difficultyLevel = difficultyLevel;
        this.coins = coins;
        this.score = score;
    }
}

[Serializable]
public class Airplane {
    public int id;
    public string name;
    public int price;
    public bool userHasIt = false;
    public AirplaneAttributes attributes;

    public Airplane(int id, string name, int price) {
        this.id = id;
        this.name = name;
        this.price = price;
        attributes = new AirplaneAttributes();
    }


    [Serializable]
    public class AirplaneAttributes {
        public float maxHealth;
        public float speed;
        public float maxSpeed;
        public float minSpeed;
        public float specialDamage;
        public float basicDamage;
    }
}
public class AirManager : MonoBehaviour {

    private Airplane airplane;
    private Airplane.AirplaneAttributes airplaneAttributes;
    private GameState gameState;
    private HealthHandler healthHandler;
    [HideInInspector] private Vector3 startedpos;
    public bool isDead { private set; get; } = false;

    private void Awake()
    {
        
        healthHandler = GetComponent<HealthHandler>();
    }

    private void Start()
    {
        startedpos = transform.position;
    }

    private void initAirplane() {
        GetComponent<AirMove>().setSpeedValues(airplaneAttributes.speed, airplaneAttributes.minSpeed, airplaneAttributes.maxSpeed);
        GetComponent<AirAttack>().setDamage(airplaneAttributes.basicDamage, airplaneAttributes.specialDamage);
        GetComponent<AirScore>().setData(gameState.score, gameState.coins);
        healthHandler.setHealth(airplaneAttributes.maxHealth, gameState.health);
        healthHandler.onDead = () => {
            isDead = true;
            airplaneDead();
        };
    }

    private void airplaneDead() {
        EventBus<AirplaneDeadEvent>.getInstance().publish(new AirplaneDeadEvent(airplane));
        transform.position = startedpos;
        initAirplane();
    }

    // Called from GameController
    public void setAirplane(Airplane airplane, GameState gameState) {
        this.airplane = airplane;
        this.airplaneAttributes = airplane.attributes;
        this.gameState = gameState;
        this.gameState.health = (int) Mathf.Clamp(gameState.health, 0f, airplane.attributes.maxHealth);
        initAirplane();
    }

    public float getCurrentHealth() {
        return healthHandler.currentHealth;
    }

    public int getCurrentScore() {
        return GetComponent<AirScore>().score;
    }

    private void OnCollisionEnter(Collision collision) {
        if (!GetComponent<AirAttack>().Shiled()) return;
        Collider collider = collision.collider;
        BaseFire fire = collider.GetComponent<BaseFire>();
        if (fire != null && !fire.createdBy.CompareTag(gameObject.tag)) {
            healthHandler.takdeDamage(fire.damage);
        }
        if (collider.CompareTag("Terrain")) {
            healthHandler.takdeDamage(10f);
        }
    }
}
public class EnemyDeadEvent : GameEvent {
    public Enemy enemy { private set; get; }

    public EnemyDeadEvent(Enemy enemy) {
        this.enemy = enemy;
    }
}
public class AirplaneDeadEvent : GameEvent {
    public Airplane airplane;

    public AirplaneDeadEvent(Airplane airplane) {
        this.airplane = airplane;
    }
}public class DifficultyChangedEvent : GameEvent {
    public Difficulty difficulty;

    public DifficultyChangedEvent(Difficulty difficulty) {
        this.difficulty = difficulty;
    }
}

