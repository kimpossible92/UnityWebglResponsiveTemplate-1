using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class obstacle : MonoBehaviour
{

    public Vector3 velocity = new Vector3(-2.5f, 0);
    public AudioClip sound;
    private Transform cachedTransform;
    private bool hasEnteredTrigger = false;
    [SerializeField] private float maxspawn = 62f, minspawn = 5f;
    [HideInInspector] private Vector3 startedpos;
    void Awake()
    {
        cachedTransform = transform;
    }
    private void Start()
    {
        transform.position = new Vector3(transform.position.x,Random.Range(minspawn, maxspawn), transform.position.z);
        startedpos = transform.position;
    }
    void Update()
    {
        cachedTransform.Translate(velocity * Time.smoothDeltaTime);
        if (!isVisible())
        {
            Deactivate();
        }
        if (transform.position.x >= 14)
        {
            transform.position = new Vector3(startedpos.x, transform.position.y, transform.position.z);
        }
        if (transform.position.x <= -14)
        {
            transform.position = new Vector3(startedpos.x, transform.position.y, transform.position.z);
        }
    }

    void OnEnable()
    {
        //cachedTransform.position = new Vector3(11, Random.Range(-3.0f, 3.0f), 5);
    }

    void OnDisable()
    {
        cachedTransform.position = new Vector3(-9999, 0, 5);
        hasEnteredTrigger = false;
    }

    bool isVisible()
    {
        bool result = true;
        Vector2 screenPosition = Camera.main.WorldToScreenPoint(cachedTransform.position);
        if (screenPosition.x < -100)
        {
            result = false;
        }
        return result;
    }

    void Deactivate()
    {
        ObjectPool.instance.PoolObject(gameObject);
    }

    void OnTriggerEnter(Collider other)
    {
        if (hasEnteredTrigger == false && other != null && other.CompareTag("Airplane"))
        {
            //Score.TotalScore += 1;
            AudioSource.PlayClipAtPoint(sound, new Vector3(0, 0, 0), 1.0f);
            hasEnteredTrigger = true;
        }
    }

}

