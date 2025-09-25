using System.Collections;
using System.Collections.Generic;
using TMPro.Examples;
using UnityEngine;

public class DreadnaughtLaserCannon : MonoBehaviour
{
    // private float _zRotation = 1;
    [SerializeField] private float _fireRate = 1.25f;
    [SerializeField] private GameObject _laserPrefab;
    [SerializeField] private int _damagePoints = 5;
    [SerializeField] private GameObject _explosionPreFab;
    private GameObject _explosion;
    private Boss _boss;
    private float _canFire = -1f;
    

    private GameObject _enemyWeapon;

    // Start is called before the first frame update
    void Start()
    {
        transform.rotation =  Quaternion.Euler (0f, 0f, 0f);
        _boss = GameObject.Find("Boss").GetComponent<Boss>();
        if (_boss == null) Debug.LogError("boss not found");
    }

    public void RotateLaserCannon(float zRotationFactor, float zRotation)
    {
        transform.Rotate(new Vector3(0, 0, zRotationFactor * zRotation), Space.Self);
        FireLaser();
    }

    public void NewRotateLaserCannon(float eulerZ)
    {
        transform.eulerAngles = new Vector3(0, 0, eulerZ);
        FireLaser();
    }
    public void ResetLaserCannonRotation(float zRotation)
    {
        transform.rotation = Quaternion.Euler(0f, 0f, zRotation);
    }

    private void FireLaser()
    {
        if (Time.time > _canFire)
        {
            _canFire = Time.time + _fireRate;
            _enemyWeapon = Instantiate(_laserPrefab, transform.position, Quaternion.identity); // spawn outside the collider
            _enemyWeapon.transform.eulerAngles = new Vector3(0f, 0f, transform.eulerAngles.z);
            _enemyWeapon.transform.parent = transform.parent.Find("Laser_Blasts");
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "PlayerLaser")
        {
            _explosion = Instantiate(_explosionPreFab, transform.position, Quaternion.identity);
            _explosion.transform.localScale = new Vector3(0.2f, 0.2f, 0.2f);
            _boss.BossDamage(_damagePoints);
            Destroy(other.gameObject);
            this.gameObject.SetActive(false);
        }
    }
}
