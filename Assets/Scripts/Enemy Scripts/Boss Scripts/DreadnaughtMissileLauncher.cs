using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class DreadnaughtMissileLauncher : MonoBehaviour
{
    private float _fireRate;
    private float _canFire = -1f;

    [SerializeField] private GameObject _missilePrefab;
    private GameObject _enemyWeapon;

    [SerializeField] private GameObject _explosionPreFab;
    private GameObject _explosion;
    private AudioSource _explosionAudioSource;
    private Boss _boss;
    [SerializeField] private int _damagePoints = 10;

    void Start()
    {
        transform.rotation = Quaternion.Euler(0f, 0f, 0f);
        _explosionAudioSource = GetComponent<AudioSource>();
        if (_explosionAudioSource == null)
        {
            Debug.LogError("Error: Explosion Audio Source not found");
        }
        _boss = GameObject.Find("Boss").GetComponent<Boss>();
        if (_boss == null) Debug.LogError("boss not found");
    }

    public void RotateMissileLauncher(float eulerZ, float fireRate, float weaponFireDelay)
    {
        transform.eulerAngles = new Vector3(0, 0, eulerZ);
        _fireRate = fireRate;
        StartCoroutine(FireWeapon(weaponFireDelay));
    }

    IEnumerator FireWeapon(float fireDelay)
    {
        yield return new WaitForSeconds(fireDelay);
        if (Time.time > _canFire)
        {
            _canFire = Time.time + _fireRate;
            _enemyWeapon = Instantiate(_missilePrefab, transform.position, Quaternion.identity); // spawn outside the collider
            _enemyWeapon.transform.eulerAngles = new Vector3(0f, 0f, transform.eulerAngles.z);
            _enemyWeapon.transform.parent = transform.parent.Find("Grenades");
        }
    }
    public void ResetMissileLauncherRotation(float zRotation)
    {
        transform.rotation = Quaternion.Euler(0f, 0f, zRotation);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "PlayerLaser")
        {
            _explosion = Instantiate(_explosionPreFab, transform.position, Quaternion.identity);
            _explosion.transform.localScale = new Vector3(0.2f, 0.2f, 0.2f);
            _explosionAudioSource.Play();
            _boss.BossDamage(_damagePoints);
            Destroy(other.gameObject);
            this.gameObject.SetActive(false);
            // Destroy(this.gameObject, .25f);
        }
    }
}
