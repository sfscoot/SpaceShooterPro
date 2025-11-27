using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CorkscrewEnemy : MonoBehaviour
{
    [SerializeField] private GameObject _explosionPreFab;
    [SerializeField] private GameObject _laserPrefab;
    [SerializeField] private int _enemyPointsValue;
    [SerializeField] private float _enemySpeed = 2.50f;

    private Player _player;
    private SpawnManager _spawnManager;
    private AudioSource _explosionAudioSource;

    private float _fireRate = 3.0f;
    private float _canFire = -1;

    private bool _enemyDestroyed = false;

    private GameObject _explosion;



    private void Start()
    {
        _player = GameObject.Find("Player").GetComponent<Player>();
        if (_player == null) Debug.LogError("Error: player object not found");

        _spawnManager = GameObject.Find("Spawn_Manager").GetComponent<SpawnManager>();
        if (_spawnManager == null) Debug.LogError("Error: SpawnManager not assigned");

        _explosionAudioSource = GetComponent<AudioSource>();
        if (_explosionAudioSource == null) Debug.LogError("Error: Explosion Audio Source not found");

        _canFire = Time.time + Random.Range(0.25f, 1.0f); // add 1 second delay before enemy can fire, otherwise they fire as soon as they're spawned
    }

    void Update()
    {
        CalculateMovement();
        if (Time.time > _canFire)
        {
            FireLaser();
        }
    }

    void CalculateMovement()
    {
        transform.Translate(Vector3.down * _enemySpeed * Time.deltaTime);
        if (transform.position.y <= -5.44f && _enemyDestroyed == false) // if moving off the bottom of screen, respawn at random x pos at top 
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
            _player.AddToScore(_enemyPointsValue);
            _player.Damage();
            PlayEnemyDeathSequence();
        }

        if (other.tag == "PlayerWeapon")
        {
            gameObject.GetComponent<Collider2D>().enabled = false; // stops the tripleshot laser getting counted twice 
            _spawnManager.GetComponent<SpawnManager>().WaveEnemyDefeated();
            Destroy(other.gameObject);
            _player.AddToScore(_enemyPointsValue);
            PlayEnemyDeathSequence();
        }
    }

    void PlayEnemyDeathSequence()
    {
        _enemyDestroyed = true;
        _explosion = Instantiate(_explosionPreFab, transform.position, Quaternion.identity);
        _explosion.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
        _enemySpeed /= 1.25f;
        Destroy(GetComponent<Collider2D>());
        Destroy(GetComponent<Rigidbody2D>());
        _canFire = Time.time + 2.8f; // make it so enemy can't fire after they've been destroyed.
        Destroy(gameObject, 0.25f);
    }

    void FireLaser()
    {
        
        _fireRate = Random.Range(3f, 7f);
        _canFire = Time.time + _fireRate;
        GameObject enemyLaser = Instantiate(_laserPrefab, transform.position, Quaternion.identity); // spawn outside the collider
        enemyLaser.transform.parent = this.transform;

        Laser[] lasers = enemyLaser.GetComponentsInChildren<Laser>();
        for (int i = 0; i < lasers.Length; i++)
        {
            lasers[i].AssignEnemyLaser();
        }
        
    }
}
