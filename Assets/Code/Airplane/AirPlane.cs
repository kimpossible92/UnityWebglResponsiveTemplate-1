using System.Collections;
using UnityEngine;

public abstract class AirPlane : MonoBehaviour
{

    // Use this for initialization
    void Start()
    {
        ProcessStart();
    }

    // Update is called once per frame
    void Update()
    {
        ProcessAttack();
    }
    protected abstract void ProcessStart();
    protected abstract void ProcessAttack();
    protected abstract void ProcessHandle();
    void FixedUpdate()
    {
        ProcessHandle();
    }
}