using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyScanner : MonoBehaviour
{
    [SerializeField] private GameObject _laserPrefab;
    private GameObject _enemyLaser;
    // private float _laserOffset = 3;
    // private GameObject _enemyLaserScript;
    private Vector3 _laserSpawnPosition;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Player")
        {
            _laserSpawnPosition = transform.parent.position;
            if (this.name == "Left_Scanner" && this.isActiveAndEnabled)
            {
                _enemyLaser = Instantiate(_laserPrefab, _laserSpawnPosition, Quaternion.identity); // spawn outside the collider
                Laser _tmpLaser = _enemyLaser.GetComponent<Laser>();
                _tmpLaser.AssignLaserDirection("left");
                _tmpLaser.AssignEnemyLaser();
                _tmpLaser.transform.Rotate(0, 0, 90, Space.Self);
                _enemyLaser.tag = "EnemyWeapon";
            } else if (this.name == "Right_Scanner" && this.isActiveAndEnabled)
            {
                _enemyLaser = Instantiate(_laserPrefab, _laserSpawnPosition, Quaternion.identity); // spawn outside the collider
                Laser _tmpLaser = _enemyLaser.GetComponent<Laser>();
                _tmpLaser.AssignLaserDirection("right");
                _tmpLaser.AssignEnemyLaser();
                _tmpLaser.transform.Rotate(0, 0, -90, Space.Self);
                _enemyLaser.tag = "EnemyWeapon";
            }
        }
    }
}
