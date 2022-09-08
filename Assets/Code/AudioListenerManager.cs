using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioListenerManager : MonoBehaviour {
    public AudioListener audioListener;
    private AudioSource AudioS;

    public void setSource(AudioSource audioSource)
    {
        this.AudioS = audioSource;
    }
    void Start() {

    }

    // Update is called once per frame
    void Update()
    {
        if (audioListener == null) return;
        bool isPaused = Pausestart.isPaused; //if(AudioS!=null)AudioS.enabled = !isPaused;

    }
}
