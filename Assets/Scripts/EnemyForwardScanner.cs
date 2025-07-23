using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class EnemyForwardScanner : MonoBehaviour
{
    [SerializeField] private GameObject _laserPrefab;
    private Vector3 _laserSpawnPosition;
    private GameObject _enemyLaser;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Powerup")
        {
            _laserSpawnPosition = transform.parent.position;
            _enemyLaser = Instantiate(_laserPrefab, _laserSpawnPosition, Quaternion.identity); // spawn outside the collider
            _enemyLaser.GetComponent<Laser>().AssignLaserDirection("down");
            _enemyLaser.GetComponent<Laser>().AssignEnemyLaser();
            // Laser _tmpLaser = _enemyLaser.GetComponent<Laser>();
            // if (_tmpLaser == null) Debug.Log("laser component not found");
            // _tmpLaser.AssignLaserDirection("down");
            // _tmpLaser.AssignEnemyLaser();
            _enemyLaser.tag = "EnemyLaser";
        }
    }
}
