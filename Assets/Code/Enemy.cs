﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class Enemy {
    private static int ID = 1;
    public int id { private set; get; }
    public float health;
    public float speed;
    public float damage;
    public float dangerZone;

    public Enemy(float health, float speed, float damage, float dangerZone) {
        this.id = ID++;
        this.health = health;
        this.speed = speed;
        this.damage = damage;
        this.dangerZone = dangerZone;
    }

    public Enemy clone() {
        return new Enemy(health, speed, damage, dangerZone);
    }
}

