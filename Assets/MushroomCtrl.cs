using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MushroomCtrl : Organisms
{
    public float moveSpeed = 0.5f;



    // Use this for initialization
    void Start()
    {
        dieAudioIndex = 14;
    }

    // Update is called once per frame

    void Update()
    {
        if (!isDie)
        {
            //moveToDir
            //transform.Translate(dir * moveSpeed * Time.deltaTime);
        }


    }

    void OnCollisionEnter(Collision other)
    {

        //dir = -dir;
    }


    public void die()
    {
        //World.playAudio(dieAudioIndex);
        GetComponent<Animator>().SetBool("isDie", true);
        isDie = true;
        Destroy(this.gameObject, 1f);
    }
}
