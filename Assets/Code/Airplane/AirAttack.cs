using Assets.Code.utils;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AirAttack : MonoBehaviour {

    public Attack basicAttack;
    public Attack specialAttack;
    private bool _shield = true;
    public bool Shiled() { return _shield; }
    [SerializeField] private GameObject Cube;
    [SerializeField]
    private Vector2 _shieldDelay;
    void Start() {
        Cube.gameObject.SetActive(false);
    }
    private IEnumerator FireDelay(float delay)
    {
        _shield = false;
        yield return new WaitForSeconds(delay);
        Cube.gameObject.SetActive(false);
        _shield = true;
    }
    // Update is called once per frame
    void Update() {
        if (basicAttack != null) {
            if (Input.GetKey(KeyCode.Mouse1)) {
                basicAttack.attack();
            }
        }

        if (specialAttack != null) {
            if (Input.GetKeyDown(KeyCode.Mouse0)) {
                specialAttack.attack();
            }
        }
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (!_shield) return;
            Cube.gameObject.SetActive(true);
            StartCoroutine(FireDelay(Random.Range(_shieldDelay.x, _shieldDelay.y)));
        }
    }

    public void setDamage(float basicDamage, float specialDamage) {
        if (basicAttack != null)
            basicAttack.damage = basicDamage;
        if (specialAttack != null)
            specialAttack.damage = specialDamage;
    }
}
