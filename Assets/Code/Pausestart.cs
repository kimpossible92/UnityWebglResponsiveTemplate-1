﻿using Assets.Code.utils;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pausestart : MonoBehaviour {

    private static Pausestart instance;

    public static bool isPaused { private set; get; } = false;

    private void Awake() {
        instance = this;
    }

    void Start()
    {
        pause();
        Cursor.visible = isPaused;
    }

    // Update is called once per frame
    void Update() {
        if (Input.GetKeyDown(KeyCode.Escape)) {
            isPaused = !isPaused;
            if (isPaused) {
                pause();
            } else {
                resume();
            }
        }
    }

    public void pause() {
        isPaused = true;
        Cursor.visible = true;
        Time.timeScale = 0f;
    }

    public void resume()
    {
        isPaused = false;
    }
    public void setContinue()
    {
        Cursor.visible = false;
        Time.timeScale = 1f;
        if (FindObjectOfType<AirManager>() != null) FindObjectOfType<AirManager>().Init2();
        if (FindObjectOfType<OpLvl>() != null)
        {

        }
    }

    private void OnDestroy() {
        resume();
        Cursor.visible = true;
    }

}
