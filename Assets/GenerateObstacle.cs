using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenerateObstacle : MonoBehaviour
{
    void Start()
    {
        StartCoroutine("CreateObstacle");
    }

    IEnumerator CreateObstacle()
    {
        float waitTime = 2.5f;
        while (true)
        {
            ObjectPool.instance.GetObjectForType("Obstacle", true);
            yield return new WaitForSeconds(waitTime);
        }
    }
}
