using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HomingMissileEnemy : MonoBehaviour
{
    [SerializeField] private SpawnManager _spawnManager;
    [SerializeField] private GameObject _explosionPreFab;
    [SerializeField] private GameObject _homingMissilePrefab;
    [SerializeField] private float _missileOffset = 1.0f;
    [SerializeField] private int _enemyPointsValue;
    [SerializeField] private float _enemySpeed = 2.50f;

    private Player _player;
    private GameObject _explosion;
    private GameObject _enemyWeapon;
    private float _fireRate = 3.0f;
    private float _canFire = -1;


    private bool _playerDestroyed;

    private void Start()
    {
        _player = GameObject.Find("Player").GetComponent<Player>();
        _spawnManager = GameObject.Find("Spawn_Manager").GetComponent<SpawnManager>();
        if (_player == null)
        {
            Debug.LogError("Player not assigned " + this.tag + "  " + this.gameObject.name);
        }
        if (_spawnManager == null)
        {
            Debug.LogError("SpawnManager not assigned");
        }

        _canFire = Time.time + Random.Range(0.25f, 1.0f); // add 1 second delay before enemy can fire, otherwise they fire as soon as they're spawned
    }

    void Update()
    {
        CalculateMovement();
        if (Time.time > _canFire)
        {
            FireMissile();
        }
    }

    void CalculateMovement()
    {
        // move  _enemySpeed meters per second
        transform.Translate(Vector3.down * _enemySpeed * Time.deltaTime);
        if (transform.position.y <= -5.44f) // if moving off the bottom of screen, respawn at random x pos at top 
        {
            float randomX = Random.Range(-9.5f, 9.5f);
            transform.position = new Vector3(randomX, 7.5f, 0);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Player")
        {
            _spawnManager.GetComponent<SpawnManager>().WaveEnemyDefeated();
            Player player = other.transform.GetComponent<Player>();

            if (_player != null)
            {
                _player.Damage();
                _player.AddToScore(_enemyPointsValue);
            }

            PlayEnemyDeathSequence();
        }

        if (other.tag == "PlayerWeapon")
        {
            gameObject.GetComponent<Collider2D>().enabled = false; // stops the tripleshot laser getting counted twice 
            _spawnManager.GetComponent<SpawnManager>().WaveEnemyDefeated();
            Destroy(other.gameObject);
            if (_player != null)
            {
                _player.AddToScore(_enemyPointsValue);
            }

            PlayEnemyDeathSequence();
        }
    }

    void PlayEnemyDeathSequence()
    {
        _explosion = Instantiate(_explosionPreFab, transform.position, Quaternion.identity);
        _explosion.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
        Destroy(gameObject);
    }

    void FireMissile()
    {
        _fireRate = Random.Range(3f, 7f);
        _canFire = Time.time + _fireRate;

        _enemyWeapon = Instantiate(_homingMissilePrefab, transform.position + new Vector3(0, _missileOffset, 0), Quaternion.identity); 
        _enemyWeapon.tag = "EnemyMissile";
    }

    public void SetPlayerDestroyed()
    {
        _playerDestroyed = true;
    }
}
