﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseMyMenu : MonoBehaviour {

    public GameObject pauseMenuPanel;

    // Start is called before the first frame update
    void Start() {

    }

    // Update is called once per frame
    void Update() {
        if (pauseMenuPanel != null) {
            bool isPaused = Pausestart.isPaused;
            if (isPaused != pauseMenuPanel.activeInHierarchy) {
                pauseMenuPanel.SetActive(isPaused);
            }
        }
    }
}
