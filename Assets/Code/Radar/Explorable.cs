using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Explorable : MonoBehaviour {

    public Image image;
    void Start() {
        //if (tag == "enemy") { print("register"); }
        Radar.getInstance().registerRadarObject(this.gameObject, image);
    }

    void OnDestroy() {
        Radar.getInstance().deregisterRadarObject(this.gameObject);
    }
}
