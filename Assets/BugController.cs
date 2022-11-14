using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BugController : MonoBehaviour
{
	public int level = 1;
	public GameObject popupMessage;
	public GameObject effect;
	public AudioSource audioSource;
	public AudioClip explodeSound;
	Vector2[] path;
	int turnNum;
	int pointNow;
	Vector3 nextPoint;
	float speed = 5;
	float angleSpeed = 180;
	int money = 100;
	int damage = 5;
	// Use this for initialization
	void Start()
	{
		turnNum = Random.Range(3, 5);
		path = new Vector2[turnNum];
		for (int i = 0; i < path.Length; i++)
			path[i] = new Vector2(Random.Range(0, Screen.width), Random.Range(0, Screen.height));
		int direction = Random.Range(1, 4);
		if (direction == 1)
			path[path.Length - 1].x = 0;
		if (direction == 2)
			path[path.Length - 1].x = Screen.width;
		if (direction == 3)
			path[path.Length - 1].y = 0;
		if (direction == 4)
			path[path.Length - 1].y = Screen.height;
		pointNow = 0;
		nextPoint = Camera.main.ScreenToWorldPoint(path[pointNow]);
		nextPoint.z = -1;
		//audioSource = GameObject.Find("Main Camera").GetComponent<PlayerInfo>().GetComponent<AudioSource>();
	}

	// Update is called once per frame
	void Update()
	{
        if (Vector3.Dot(transform.forward.normalized, (transform.position - nextPoint).normalized) > Mathf.Cos(3))
            transform.LookAt(nextPoint);
        if (Vector3.Dot(transform.forward.normalized, (nextPoint - transform.position).normalized) < Mathf.Cos(0.1f * Mathf.PI))
		{
			if (Vector3.Angle(Vector3.forward, (nextPoint - transform.position).normalized) < 180)
				transform.RotateAround(transform.position, transform.up, angleSpeed * Time.deltaTime);
			else
				transform.RotateAround(transform.position, -transform.up, angleSpeed * Time.deltaTime);
		}
		else
			transform.position = Vector3.MoveTowards(transform.position, nextPoint, Time.deltaTime * speed);
		if (Vector3.Distance(transform.position, nextPoint) < 1)
		{
			pointNow++;
			if (pointNow < path.Length)
			{
				nextPoint = Camera.main.ScreenToWorldPoint(path[pointNow]);
				nextPoint.z = -1;
			}
			else
			{
				Instantiate(effect, transform.position, Quaternion.identity);
				audioSource.PlayOneShot(explodeSound);
				Destroy(transform.gameObject);
				//PlayerInfo.hp -= damage;
			}
		}
	}

	public void hit(int combo)
	{
		money *= combo;
		Instantiate(effect, transform.position, Quaternion.identity);
		Vector2 position2d = Camera.main.WorldToScreenPoint(transform.position);
		GameObject obj = Instantiate(popupMessage, transform.position, Quaternion.identity) as GameObject;
		//PopupMessage popup = obj.GetComponent<PopupMessage>();
		//popup.position2d = position2d;
		//popup.message = "+" + money;
		//PlayerInfo.money += money;
		audioSource.PlayOneShot(explodeSound);
		Destroy(transform.gameObject);
	}

	public void setLevel(int lv)
	{
		level = lv;
		speed *= level;
		angleSpeed *= level;
		money *= level;
		damage *= level;
	}

	void OnMouseDown()
	{
		//InputController inputController = GameObject.Find("ControlPane").GetComponent<InputController>();
		//inputController.fire();
	}

	void OnMouseDrag()
	{
		//InputController inputController = GameObject.Find("ControlPane").GetComponent<InputController>();
		//inputController.fire();
	}
}
