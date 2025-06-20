using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyScanner : MonoBehaviour
{
    [SerializeField] private GameObject _laserPrefab;
    private GameObject _enemyLaser;
    private GameObject _enemyLaserScript;


    private void Start()
    {

    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Player")
        {
            Debug.Log("player is following " + this.name);
            if (this.name == "Left_Scanner")
            {
                _enemyLaser = Instantiate(_laserPrefab, transform.position, Quaternion.identity); // spawn outside the collider
                Laser _tmpLaser = _enemyLaser.GetComponent<Laser>();
                _tmpLaser.AssignLaserDirection("left");
                _enemyLaser.tag = "EnemyLaser";
            }
            if (this.name == "Right_Scanner")
            {
                _enemyLaser = Instantiate(_laserPrefab, transform.position, Quaternion.identity); // spawn outside the collider
                Laser _tmpLaser = _enemyLaser.GetComponent<Laser>();
                _tmpLaser.AssignLaserDirection("right");
                _enemyLaser.tag = "EnemyLaser";
            }
            Debug.Log("should have fired a laser");
        }
    }
}
