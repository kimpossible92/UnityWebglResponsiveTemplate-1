﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Session {
    public int id;
    public int airplaneId;
    public int environmentId;
    public string sessionName;
    public GameStatest gameState;

    public Session(int airplaneId, int environmentId, string sessionName, GameStatest gameState) {
        this.id = DateTime.Now.Millisecond;
        this.airplaneId = airplaneId;
        this.environmentId = environmentId;
        this.sessionName = sessionName;
        this.gameState = gameState;
    }

}

[Serializable]
public class GameStatest {
    public float health;
    public int difficultyLevel;
    public int coins;
    public int score;

    public GameStatest(float health, int difficultyLevel, int coins, int score) {
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
public enum slojnost2
{
    easy=1,med=2,hard=3
}
public class AirManager : MonoBehaviour {

    private Airplane airplane;
   
    private Airplane.AirplaneAttributes airplaneAttributes;
    private GameStatest gameState;
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
        live = PlayerPrefs.GetInt("live1");
    }
    private void Update()
    {
        if (transform.position.z > 3440)
        {
            transform.position = startedpos;
        }
        if (transform.position.x >= 14)
        {
            transform.position = new Vector3(startedpos.x,transform.position.y,transform.position.z);
        }
        if(transform.position.x <= -14)
        {
            transform.position = new Vector3(startedpos.x, transform.position.y, transform.position.z);
        }
    }

    private void initAirplane() {
        GetComponent<AirMove>().setSpeedValues(airplaneAttributes.speed, airplaneAttributes.minSpeed, airplaneAttributes.maxSpeed);
        GetComponent<AirMove>().setDamage(airplaneAttributes.basicDamage, airplaneAttributes.specialDamage);
        GetComponent<AirScore>().setData(gameState.score, gameState.coins);
        healthHandler.setHealth(airplaneAttributes.maxHealth, gameState.health);
        healthHandler.onDead = () => {
            //print(isDead);
            isDead = true;
            airplaneDead();
        };
    }int live = 0;
    private void OnApplicationQuit()
    {
        PlayerPrefs.SetInt("live1", live);
        PlayerPrefs.Save();
    }
    private void OnGUI()
    {
        GUIStyle style = new GUIStyle();
        style.fontSize = 30;
        style.fontStyle = FontStyle.Bold;
        style.normal.textColor = Color.yellow;
        Rect lifeIconRect = new Rect(10, 150, 32, 32);
        Rect labelRect = new Rect(lifeIconRect.xMax + 10, lifeIconRect.y, 60, 32);
        GUI.Label(labelRect, live.ToString(), style);
    }
    public void Init2()
    {
        if (isDead)
        {
            initAirplane();
            live++;
            //print("resume");
            isDead = false;
        }
    }
    private void airplaneDead() {
        EventBus<AirplaneDeadEvent>.getInstance().publish(new AirplaneDeadEvent(airplane));
        //transform.position = startedpos;
        initAirplane();
    }

    // Called from Gamestart
    public void setAirplane(Airplane airplane, GameStatest gameState) {
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
        if (!GetComponent<AirMove>().Shiled()) return;
        Collider collider = collision.collider;
        BaseFire fire = collider.GetComponent<BaseFire>();
        if (fire != null && !fire.createdBy.CompareTag(gameObject.tag)) {
            //healthHandler.takdeDamage(fire.damage);
        }
        if (collider.CompareTag("Terrain")) {
            healthHandler.takdeDamage(10f);
            transform.position = new Vector3(transform.position.x, startedpos.y, transform.position.z);
        }
        if (collider.CompareTag("enemy"))
        {
            healthHandler.takdeDamage(10f);
            transform.position = new Vector3(transform.position.x, startedpos.y, transform.position.z);
        }
        if (healthHandler.currentHealth <= 0)
        {

        }
        if (collider.CompareTag("bonus"))
        {
            if (collider.GetComponent<Gameplay.Spaceships.Spaceship>() != null)
            {
                if (collider.GetComponent<Gameplay.Spaceships.Spaceship>().bonusRead == 0)
                {
                    healthHandler.plusLive(20f);Destroy(collider.gameObject);
                }
                if (collider.GetComponent<Gameplay.Spaceships.Spaceship>().bonusRead == 1)
                {
                    healthHandler.plusLive(40f); Destroy(collider.gameObject);
                }
                if (collider.GetComponent<Gameplay.Spaceships.Spaceship>().bonusRead == 2)
                {
                    healthHandler.plusLive(60f); Destroy(collider.gameObject);
                }
                else
                {
                    healthHandler.plusLive(60f); Destroy(collider.gameObject);
                }
            }
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

