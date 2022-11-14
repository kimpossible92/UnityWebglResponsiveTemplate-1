using System;
using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Gameplay.Spawners
{
    public class Spawner : MonoBehaviour
    {

        [SerializeField]
        private GameObject _object;
        int level=0;
        [SerializeField]
        private Transform _parent;
        
        [SerializeField]
        private Vector2 _spawnPeriodRange;
        
        [SerializeField]
        private Vector2 _spawnDelayRange;

        [SerializeField]
        private bool _autoStart = true;
        [SerializeField]
        private Sprite[] Objects;
        [SerializeField] private int anothermovement = 0;
        [SerializeField] private Material GetMaterial;
        [HideInInspector] private bool _ismyShip;
        public void setNewShip(bool shipB) { _ismyShip = shipB; }
        public void lvlplus()
        {
            level++;
        }
        private void Start()
        {
            _ismyShip = false;
            //if (_autoStart)
                StartSpawn();
        }
        public void NoSpawnStart() { _autoStart = false; }

        public void StartSpawn()
        {
            StartCoroutine(Spawn());
        }

        public void StopSpawn()
        {
            StopAllCoroutines();
        }


        private IEnumerator Spawn()
        {
            yield return new WaitForSeconds(Random.Range(_spawnDelayRange.x, _spawnDelayRange.y));

            while (true)
            {
                //bool ismyShip = false;
                //foreach(var ship1 in FindObjectsOfType<EnemySp>()) { if (ship1.GetSpawner == this) { ismyShip = true; } }
               
                if (!_ismyShip)
                {
                    var enem = Instantiate(_object, transform.position, Quaternion.identity);
                    if (23 >= level)
                    {
                        enem.transform.Find("Hull").GetComponent<SpriteRenderer>().sprite = Objects[level];
                        //var ptexure = Objects[level].texture;
                        //ptexure.SetPixel()
                        //enem.transform.Find("Hull2").GetComponent<Renderer>().material.mainTexture = Objects[level].texture;
                        if (level >= 8) { enem.GetComponent<Spaceships.Spaceship>().setlive(200f); enem.GetComponent<Gameplay.ShipSystems.WeaponSystem>().setRocketOrBeam(); }
                        else if (level >= 16) { enem.GetComponent<Spaceships.Spaceship>().setlive(300f); enem.GetComponent<Gameplay.ShipSystems.WeaponSystem>().setBeam(); }
                        else if (level >= 21) { enem.GetComponent<Spaceships.Spaceship>().setlive(400f); enem.GetComponent<Gameplay.ShipSystems.WeaponSystem>().setBeam(); }
                        else { enem.GetComponent<Spaceships.Spaceship>().setlive(1f); }
                    }
                    else { level = 0; }
                    enem.GetComponent<EnemySp>().GetSpawner = this;
                    if (anothermovement == 1) { enem.GetComponent<EnemyShipController>().setAnotherMovement(1); }
                    if (anothermovement == -1) { enem.GetComponent<EnemyShipController>().setAnotherMovement(-1); }
                    else { }
                    //print(enem.transform.position);
                    _ismyShip =true;
                }
                yield return new WaitForSeconds(Random.Range(_spawnPeriodRange.x, _spawnPeriodRange.y));
            }
        }
    }
}
