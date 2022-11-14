using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParentFire : MonoBehaviour
{
    private bool isUp = false;
    //private Vector3 DestroyPosition;
    // Start is called before the first frame update
    public void SetUp(bool _isup) { isUp = _isup; }
    void Start()
    {
        
    }
    public void newAnim()
    {
        StartCoroutine(GetEnumerator());
    }
    IEnumerator GetEnumerator()
    {
        yield return new WaitForSeconds(5.5f);
        Destroy(gameObject);
    }
    // Update is called once per frame
    void Update()
    {
        if(isUp)transform.Translate(0.5f * Vector3.up * Time.deltaTime);
    }
}
